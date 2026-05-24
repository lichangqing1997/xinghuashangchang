using SqlSugar;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.ProductService;

public class ProductService : IProductService
{
    private readonly ISqlSugarClient _db;

    public ProductService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<Product> Items, int Total)> GetList(string? keyword = null, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<Product>().Where(x => x.Status == 1);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.ProductName.Contains(keyword) || x.Barcode.Contains(keyword) || x.ProductCode.Contains(keyword));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Product?> GetById(int id)
    {
        return await _db.Queryable<Product>()
            .Where(x => x.Id == id && x.Status == 1)
            .FirstAsync();
    }

    public async Task<Product?> GetByBarcode(string barcode)
    {
        return await _db.Queryable<Product>()
            .Where(x => x.Barcode == barcode && x.Status == 1)
            .FirstAsync();
    }

    public async Task<int> Add(Product entity)
    {
        var existing = await _db.Queryable<Product>().Where(x => x.Barcode == entity.Barcode).FirstAsync();
        if (existing != null)
            throw new InvalidOperationException("该条形码已存在");

        entity.CreatedAt = DateTime.UtcNow;
        return await _db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    public async Task<bool> Update(Product entity)
    {
        var existing = await _db.Queryable<Product>()
            .Where(x => x.Barcode == entity.Barcode && x.Id != entity.Id)
            .FirstAsync();
        if (existing != null)
            throw new InvalidOperationException("该条形码已被其他商品使用");

        entity.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await GetById(id);
        if (entity == null)
            return false;

        var inventoryCount = await _db.Queryable<Inventory>().Where(x => x.ProductId == id).CountAsync();
        if (inventoryCount > 0)
            throw new InvalidOperationException("该商品还有库存，无法删除");

        entity.Status = 0;
        entity.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(entity).UpdateColumns(x => new { x.Status, x.UpdatedAt }).ExecuteCommandHasChangeAsync();
    }
}
