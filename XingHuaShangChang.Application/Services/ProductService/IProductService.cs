using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.ProductService;

/// <summary>
/// 商品服务接口
/// </summary>
public interface IProductService
{
    Task<(List<Product> Items, int Total)> GetList(string? keyword = null, int pageIndex = 1, int pageSize = 20);
    Task<Product?> GetById(int id);
    Task<Product?> GetByBarcode(string barcode);
    Task<int> Add(Product entity);
    Task<bool> Update(Product entity);
    Task<bool> Delete(int id);
}
