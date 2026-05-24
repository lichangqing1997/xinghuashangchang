using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.PDA;

public class PdaService : IPdaService
{
    private readonly ISqlSugarClient _db;
    private const string TempLocationCode = "TEMP";

    public PdaService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<string> ScanIn(PdaScanInInput input)
    {
        try
        {
            _db.Ado.BeginTran();

            var product = await _db.Queryable<Product>().Where(x => x.Barcode == input.Barcode).FirstAsync();
            if (product == null)
            {
                _db.Ado.RollbackTran();
                return "未找到该条形码对应的商品";
            }

            var location = await _db.Queryable<Location>().Where(x => x.LocationCode == input.LocationCode).FirstAsync();
            if (location == null)
            {
                _db.Ado.RollbackTran();
                return "未找到该库位";
            }

            var existingInventory = await _db.Queryable<Inventory>()
                .Where(x => x.ProductId == product.Id && x.LocationId == location.Id)
                .FirstAsync();

            if (existingInventory != null)
            {
                var affected = await _db.Updateable<Inventory>()
                    .SetColumns(x => new Inventory { Quantity = x.Quantity + input.Quantity, UpdatedAt = DateTime.Now })
                    .Where(x => x.Id == existingInventory.Id)
                    .ExecuteCommandAsync();

                if (affected == 0)
                {
                    _db.Ado.RollbackTran();
                    return "库存更新失败，请重试";
                }
            }
            else
            {
                var newInventory = new Inventory
                {
                    ProductId = product.Id,
                    LocationId = location.Id,
                    Quantity = input.Quantity
                };
                await _db.Insertable(newInventory).ExecuteReturnIdentityAsync();
            }

            location.Status = "占用";
            await _db.Updateable(location).ExecuteCommandHasChangeAsync();

            var record = new ScanRecord
            {
                Barcode = input.Barcode,
                ProductId = product.Id,
                OperationType = "In",
                LocationId = location.Id,
                Quantity = input.Quantity,
                ReferenceNo = input.PurchaseOrderNo,
                Operator = input.Operator
            };
            await _db.Insertable(record).ExecuteReturnIdentityAsync();

            _db.Ado.CommitTran();
            return "入库成功";
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<string> ScanOut(PdaScanOutInput input)
    {
        try
        {
            _db.Ado.BeginTran();

            var product = await _db.Queryable<Product>().Where(x => x.Barcode == input.Barcode).FirstAsync();
            if (product == null)
            {
                _db.Ado.RollbackTran();
                return "未找到该条形码对应的商品";
            }

            var inventory = await _db.Queryable<Inventory>()
                .Where(x => x.ProductId == product.Id && x.Quantity - x.LockedQuantity >= input.Quantity)
                .OrderBy(x => x.CreatedAt)
                .FirstAsync();

            if (inventory == null)
            {
                _db.Ado.RollbackTran();
                return "库存不足或未找到库存";
            }

            var affected = await _db.Updateable<Inventory>()
                .SetColumns(x => new Inventory { Quantity = x.Quantity - input.Quantity, UpdatedAt = DateTime.Now })
                .Where(x => x.Id == inventory.Id && x.Quantity >= input.Quantity)
                .ExecuteCommandAsync();

            if (affected == 0)
            {
                _db.Ado.RollbackTran();
                return "库存扣减失败，可能已被其他操作修改，请重试";
            }

            var updatedInventory = await _db.Queryable<Inventory>().Where(x => x.Id == inventory.Id).FirstAsync();
            if (updatedInventory != null && updatedInventory.Quantity <= 0)
            {
                await _db.Deleteable(updatedInventory).ExecuteCommandHasChangeAsync();
            }

            var locationInventoryCount = await _db.Queryable<Inventory>()
                .Where(x => x.LocationId == inventory.LocationId)
                .CountAsync();

            if (locationInventoryCount == 0)
            {
                var location = await _db.Queryable<Location>().Where(x => x.Id == inventory.LocationId).FirstAsync();
                if (location != null)
                {
                    location.Status = "空闲";
                    await _db.Updateable(location).ExecuteCommandHasChangeAsync();
                }
            }

            var record = new ScanRecord
            {
                Barcode = input.Barcode,
                ProductId = product.Id,
                OperationType = "Out",
                LocationId = inventory.LocationId,
                Quantity = input.Quantity,
                ReferenceNo = input.SalesOrderNo,
                Operator = input.Operator
            };
            await _db.Insertable(record).ExecuteReturnIdentityAsync();

            _db.Ado.CommitTran();
            return "出库成功";
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<string> ShelfConfirm(PdaShelfInput input)
    {
        return await ScanIn(new PdaScanInInput
        {
            Barcode = input.Barcode,
            LocationCode = input.LocationCode,
            Quantity = input.Quantity,
            Operator = input.Operator
        });
    }

    public async Task<string> MobileReceive(MobileReceiveInput input)
    {
        if (!string.IsNullOrEmpty(input.Barcode))
        {
            return await ScanIn(new PdaScanInInput
            {
                Barcode = input.Barcode,
                LocationCode = TempLocationCode,
                Quantity = input.Quantity,
                PurchaseOrderNo = input.PurchaseOrderNo,
                Operator = input.Operator
            });
        }

        return "请扫描商品条形码";
    }

    public async Task<(List<ScanRecord> Items, int Total)> GetScanRecords(
        string? barcode = null,
        string? operationType = null,
        int pageIndex = 1,
        int pageSize = 20)
    {
        var query = _db.Queryable<ScanRecord>();
        if (!string.IsNullOrEmpty(barcode))
            query = query.Where(x => x.Barcode == barcode);
        if (!string.IsNullOrEmpty(operationType))
            query = query.Where(x => x.OperationType == operationType);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.ScanTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
