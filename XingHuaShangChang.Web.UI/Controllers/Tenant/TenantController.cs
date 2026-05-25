using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.TenantService;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.Tenant;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _service;

    public TenantController(ITenantService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<Result<PageResult<Application.Entity.Tenant>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var (items, total) = await _service.GetList(keyword, pageIndex, pageSize);
        var pageResult = new PageResult<Application.Entity.Tenant>
        {
            Items = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Result<PageResult<Application.Entity.Tenant>>.Ok(pageResult);
    }

    [HttpGet("{id}")]
    public async Task<Result<Application.Entity.Tenant>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return Result<Application.Entity.Tenant>.Fail("未找到租户", "NOT_FOUND");
        return Result<Application.Entity.Tenant>.Ok(entity);
    }

    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateTenantInput input)
    {
        try
        {
            var id = await _service.Create(input);
            return Result<int>.Ok(id, "创建成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<int>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }

    [HttpPut]
    public async Task<Result<bool>> Update([FromBody] UpdateTenantInput input)
    {
        try
        {
            var result = await _service.Update(input);
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
