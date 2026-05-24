using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Services.ProductService;
using XingHuaShangChang.Common.Util;
using ProductEntity = XingHuaShangChang.Application.Entity.Product;

namespace XingHuaShangChang.Web.UI.Controllers.Product;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<Result<PageResult<ProductEntity>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var (items, total) = await _service.GetList(keyword, pageIndex, pageSize);
        var pageResult = new PageResult<ProductEntity>
        {
            Items = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Result<PageResult<ProductEntity>>.Ok(pageResult);
    }

    [HttpGet("{id}")]
    public async Task<Result<ProductEntity>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return Result<ProductEntity>.Fail("未找到商品", "NOT_FOUND");
        return Result<ProductEntity>.Ok(entity);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<Result<ProductEntity>> GetByBarcode(string barcode)
    {
        var entity = await _service.GetByBarcode(barcode);
        if (entity == null)
            return Result<ProductEntity>.Fail("未找到商品", "NOT_FOUND");
        return Result<ProductEntity>.Ok(entity);
    }

    [HttpPost]
    public async Task<Result<int>> Add([FromBody] ProductEntity entity)
    {
        try
        {
            var id = await _service.Add(entity);
            return Result<int>.Ok(id, "新增成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<int>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }

    [HttpPut]
    public async Task<Result<bool>> Update([FromBody] ProductEntity entity)
    {
        try
        {
            var result = await _service.Update(entity);
            return result ? Result<bool>.Ok(true, "更新成功") : Result<bool>.Fail("更新失败");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }

    [HttpDelete("{id}")]
    public async Task<Result<bool>> Delete(int id)
    {
        try
        {
            var result = await _service.Delete(id);
            return result ? Result<bool>.Ok(true, "删除成功") : Result<bool>.Fail("删除失败");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }
}
