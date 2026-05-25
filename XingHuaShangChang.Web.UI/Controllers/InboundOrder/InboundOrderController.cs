using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.InboundOrderService;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.InboundOrder;

[ApiController]
[Route("api/[controller]")]
public class InboundOrderController : ControllerBase
{
    private readonly IInboundOrderService _service;

    public InboundOrderController(IInboundOrderService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<Result<PageResult<Application.Entity.InboundOrder>>> GetList([FromQuery] InboundOrderQueryInput query)
    {
        query.PageSize = Math.Min(query.PageSize, 100);
        var (items, total) = await _service.GetList(query);
        var pageResult = new PageResult<Application.Entity.InboundOrder>
        {
            Items = items,
            Total = total,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
        return Result<PageResult<Application.Entity.InboundOrder>>.Ok(pageResult);
    }

    [HttpGet("{id}")]
    public async Task<Result<Application.Entity.InboundOrder>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return Result<Application.Entity.InboundOrder>.Fail("未找到入库单", "NOT_FOUND");
        return Result<Application.Entity.InboundOrder>.Ok(entity);
    }

    [HttpGet("{id}/items")]
    public async Task<Result<List<InboundOrderItem>>> GetItems(int id)
    {
        var items = await _service.GetItemsByOrderId(id);
        return Result<List<InboundOrderItem>>.Ok(items);
    }

    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateInboundOrderInput input)
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
    public async Task<Result<bool>> Update([FromBody] UpdateInboundOrderInput input)
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

    [HttpPost("{id}/audit")]
    public async Task<Result<bool>> Audit(int id, [FromBody] AuditInboundOrderInput input)
    {
        try
        {
            var result = await _service.Audit(id, input);
            return result ? Result<bool>.Ok(true, "审核成功") : Result<bool>.Fail("审核失败");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }

    [HttpPost("{id}/change-status")]
    public async Task<Result<bool>> ChangeStatus(int id, [FromQuery] string newStatus, [FromQuery] string? operatorName)
    {
        try
        {
            var result = await _service.ChangeStatus(id, newStatus, operatorName);
            return result ? Result<bool>.Ok(true, "状态变更成功") : Result<bool>.Fail("状态变更失败");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }
}
