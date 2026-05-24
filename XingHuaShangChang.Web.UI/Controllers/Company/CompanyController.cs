using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.CompanyService;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.Company;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompanyController(ICompanyService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<Result<PageResult<Application.Entity.Company>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var (items, total) = await _service.GetList(keyword, pageIndex, pageSize);
        var pageResult = new PageResult<Application.Entity.Company>
        {
            Items = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Result<PageResult<Application.Entity.Company>>.Ok(pageResult);
    }

    [HttpGet("{id}")]
    public async Task<Result<Application.Entity.Company>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return Result<Application.Entity.Company>.Fail("未找到公司", "NOT_FOUND");
        return Result<Application.Entity.Company>.Ok(entity);
    }

    [HttpGet("{id}/details")]
    public async Task<Result<List<CompanyDetail>>> GetDetails(int id)
    {
        var details = await _service.GetDetailsByCompanyId(id);
        return Result<List<CompanyDetail>>.Ok(details);
    }

    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateCompanyInput input)
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
    public async Task<Result<bool>> Update([FromBody] UpdateCompanyInput input)
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
