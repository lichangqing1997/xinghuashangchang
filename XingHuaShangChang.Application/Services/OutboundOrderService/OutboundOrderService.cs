using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.OutboundOrderService;

public class OutboundOrderService : IOutboundOrderService
{
    private readonly ISqlSugarClient _db;

    public OutboundOrderService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<OutboundOrder> Items, int Total)> GetList(OutboundOrderQueryInput query)
    {
        var q = _db.Queryable<OutboundOrder>();

        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(x => x.OrderNo.Contains(query.Keyword) || (x.CompanyName != null && x.CompanyName.Contains(query.Keyword)));
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(x => x.Status == query.Status);
        if (query.StartDate.HasValue)
            q = q.Where(x => x.CreatedAt >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            q = q.Where(x => x.CreatedAt < query.EndDate.Value.AddDays(1));

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<OutboundOrder?> GetById(int id)
    {
        return await _db.Queryable<OutboundOrder>().InSingleAsync(id);
    }

    public async Task<List<OutboundOrderItem>> GetItemsByOrderId(int orderId)
    {
        return await _db.Queryable<OutboundOrderItem>()
            .Where(x => x.OutboundOrderId == orderId)
            .ToListAsync();
    }

    public async Task<int> Create(CreateOutboundOrderInput input)
    {
        if (!input.Items.Any())
            throw new InvalidOperationException("至少需要一行出库明细");

        try
        {
            _db.Ado.BeginTran();

            // 生成出库单号
            var orderNo = await GenerateOrderNo();

            // 快照商品信息并计算金额
            var items = new List<OutboundOrderItem>();
            decimal totalAmount = 0;

            foreach (var itemInput in input.Items)
            {
                var product = await _db.Queryable<Product>().Where(x => x.Id == itemInput.ProductId && x.Status == 1).FirstAsync();
                if (product == null)
                    throw new InvalidOperationException($"商品ID {itemInput.ProductId} 不存在");

                var amount = itemInput.UnitPrice * itemInput.Quantity;
                totalAmount += amount;

                items.Add(new OutboundOrderItem
                {
                    ProductId = itemInput.ProductId,
                    Specification = product.Specification,
                    UnitPrice = itemInput.UnitPrice,
                    Quantity = itemInput.Quantity,
                    Amount = amount,
                    Manufacturer = product.Manufacturer,
                    ExpiryDate = itemInput.ExpiryDate,
                    ProductionDate = itemInput.ProductionDate,
                    BatchNo = itemInput.BatchNo,
                    RegistrationNo = product.RegistrationNo,
                    LocationId = itemInput.LocationId,
                    CompanyName = itemInput.CompanyName
                });
            }

            // 插入出库单
            var order = new OutboundOrder
            {
                OrderNo = orderNo,
                CompanyName = input.CompanyName,
                TotalAmount = totalAmount,
                Status = "待出库",
                Operator = input.Operator,
                Remark = input.Remark
            };
            var orderId = await _db.Insertable(order).ExecuteReturnIdentityAsync();

            // 插入明细行
            foreach (var item in items)
            {
                item.OutboundOrderId = orderId;
            }
            await _db.Insertable(items).ExecuteCommandAsync();

            _db.Ado.CommitTran();
            return orderId;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<bool> Update(UpdateOutboundOrderInput input)
    {
        var order = await _db.Queryable<OutboundOrder>().InSingleAsync(input.Id);
        if (order == null)
            throw new InvalidOperationException("出库单不存在");
        if (order.Status != "待出库")
            throw new InvalidOperationException("只能编辑待出库状态的出库单");

        if (!input.Items.Any())
            throw new InvalidOperationException("至少需要一行出库明细");

        try
        {
            _db.Ado.BeginTran();

            // 快照商品信息并计算金额
            var items = new List<OutboundOrderItem>();
            decimal totalAmount = 0;

            foreach (var itemInput in input.Items)
            {
                var product = await _db.Queryable<Product>().Where(x => x.Id == itemInput.ProductId && x.Status == 1).FirstAsync();
                if (product == null)
                    throw new InvalidOperationException($"商品ID {itemInput.ProductId} 不存在");

                var amount = itemInput.UnitPrice * itemInput.Quantity;
                totalAmount += amount;

                items.Add(new OutboundOrderItem
                {
                    OutboundOrderId = input.Id,
                    ProductId = itemInput.ProductId,
                    Specification = product.Specification,
                    UnitPrice = itemInput.UnitPrice,
                    Quantity = itemInput.Quantity,
                    Amount = amount,
                    Manufacturer = product.Manufacturer,
                    ExpiryDate = itemInput.ExpiryDate,
                    ProductionDate = itemInput.ProductionDate,
                    BatchNo = itemInput.BatchNo,
                    RegistrationNo = product.RegistrationNo,
                    LocationId = itemInput.LocationId,
                    CompanyName = itemInput.CompanyName
                });
            }

            // 更新出库单头
            order.CompanyName = input.CompanyName;
            order.TotalAmount = totalAmount;
            order.Operator = input.Operator;
            order.Remark = input.Remark;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.Updateable(order).ExecuteCommandHasChangeAsync();

            // 删除旧明细，插入新明细
            await _db.Deleteable<OutboundOrderItem>().Where(x => x.OutboundOrderId == input.Id).ExecuteCommandAsync();
            await _db.Insertable(items).ExecuteCommandAsync();

            _db.Ado.CommitTran();
            return true;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        var order = await _db.Queryable<OutboundOrder>().InSingleAsync(id);
        if (order == null)
            throw new InvalidOperationException("出库单不存在");
        if (order.Status != "待出库")
            throw new InvalidOperationException("只能取消待出库状态的出库单");

        order.Status = "已取消";
        order.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(order).UpdateColumns(x => new { x.Status, x.UpdatedAt }).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> ConfirmOutbound(int orderId, string? operatorName)
    {
        var order = await _db.Queryable<OutboundOrder>().InSingleAsync(orderId);
        if (order == null)
            throw new InvalidOperationException("出库单不存在");
        if (order.Status != "待出库")
            throw new InvalidOperationException("该出库单状态不允许出库确认");

        var items = await _db.Queryable<OutboundOrderItem>()
            .Where(x => x.OutboundOrderId == orderId)
            .ToListAsync();

        if (!items.Any())
            throw new InvalidOperationException("出库单明细为空");

        try
        {
            _db.Ado.BeginTran();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (!item.LocationId.HasValue)
                    throw new InvalidOperationException($"第{i + 1}行库位未指定");

                // 查找库存
                var inventory = await _db.Queryable<Inventory>()
                    .Where(x => x.ProductId == item.ProductId && x.LocationId == item.LocationId.Value)
                    .FirstAsync();

                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    var product = await _db.Queryable<Product>().Where(x => x.Id == item.ProductId).FirstAsync();
                    var location = await _db.Queryable<Location>().Where(x => x.Id == item.LocationId.Value).FirstAsync();
                    throw new InvalidOperationException($"商品{product?.ProductName}在库位{location?.LocationCode}库存不足");
                }

                // 乐观并发扣减库存
                var affected = await _db.Updateable<Inventory>()
                    .SetColumns(x => new Inventory { Quantity = x.Quantity - item.Quantity, UpdatedAt = DateTime.Now })
                    .Where(x => x.Id == inventory.Id && x.Quantity >= item.Quantity)
                    .ExecuteCommandAsync();

                if (affected == 0)
                    throw new InvalidOperationException("库存扣减失败，请重试");

                // 若库存归零则删除
                var updatedInventory = await _db.Queryable<Inventory>().Where(x => x.Id == inventory.Id).FirstAsync();
                if (updatedInventory != null && updatedInventory.Quantity <= 0)
                {
                    await _db.Deleteable(updatedInventory).ExecuteCommandHasChangeAsync();
                }

                // 若该库位无库存则设为空闲
                var locationInventoryCount = await _db.Queryable<Inventory>()
                    .Where(x => x.LocationId == item.LocationId.Value)
                    .CountAsync();

                if (locationInventoryCount == 0)
                {
                    var location = await _db.Queryable<Location>().Where(x => x.Id == item.LocationId.Value).FirstAsync();
                    if (location != null)
                    {
                        location.Status = "空闲";
                        await _db.Updateable(location).ExecuteCommandHasChangeAsync();
                    }
                }

                // 插入出库流水
                var productInfo = await _db.Queryable<Product>().Where(x => x.Id == item.ProductId).FirstAsync();
                var locationInfo = await _db.Queryable<Location>().Where(x => x.Id == item.LocationId.Value).FirstAsync();

                var flow = new OutboundFlow
                {
                    OrderNo = order.OrderNo,
                    ProductId = item.ProductId,
                    ProductName = productInfo?.ProductName,
                    ProductCode = productInfo?.ProductCode,
                    LocationId = item.LocationId.Value,
                    LocationCode = locationInfo?.LocationCode,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Amount = item.Amount,
                    BatchNo = item.BatchNo,
                    Operator = operatorName
                };
                await _db.Insertable(flow).ExecuteReturnIdentityAsync();
            }

            // 更新出库单状态
            order.Status = "已出库";
            order.OutboundAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.Updateable(order).UpdateColumns(x => new { x.Status, x.OutboundAt, x.UpdatedAt }).ExecuteCommandHasChangeAsync();

            _db.Ado.CommitTran();
            return true;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<(List<OutboundFlow> Items, int Total)> GetFlowList(string? orderNo, string? keyword, int pageIndex = 1, int pageSize = 20)
    {
        var q = _db.Queryable<OutboundFlow>();

        if (!string.IsNullOrEmpty(orderNo))
            q = q.Where(x => x.OrderNo == orderNo);
        if (!string.IsNullOrEmpty(keyword))
            q = q.Where(x => (x.ProductName != null && x.ProductName.Contains(keyword)) || (x.ProductCode != null && x.ProductCode.Contains(keyword)));

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(x => x.FlowTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    private async Task<string> GenerateOrderNo()
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var prefix = $"CK{today}";

        var lastOrder = await _db.Queryable<OutboundOrder>()
            .Where(x => x.OrderNo.StartsWith(prefix))
            .OrderByDescending(x => x.OrderNo)
            .FirstAsync();

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastSeq = lastOrder.OrderNo.Substring(prefix.Length);
            if (int.TryParse(lastSeq, out int s))
                sequence = s + 1;
        }

        return $"{prefix}{sequence:D4}";
    }
}
