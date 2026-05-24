using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.OutboundOrderService;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.OutboundOrder;

[ApiController]
[Route("api/[controller]")]
public class OutboundOrderController : ControllerBase
{
    private readonly IOutboundOrderService _service;

    public OutboundOrderController(IOutboundOrderService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<Result<PageResult<Application.Entity.OutboundOrder>>> GetList([FromQuery] OutboundOrderQueryInput query)
    {
        query.PageSize = Math.Min(query.PageSize, 100);
        var (items, total) = await _service.GetList(query);
        var pageResult = new PageResult<Application.Entity.OutboundOrder>
        {
            Items = items,
            Total = total,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
        return Result<PageResult<Application.Entity.OutboundOrder>>.Ok(pageResult);
    }

    [HttpGet("{id}")]
    public async Task<Result<Application.Entity.OutboundOrder>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return Result<Application.Entity.OutboundOrder>.Fail("未找到出库单", "NOT_FOUND");
        return Result<Application.Entity.OutboundOrder>.Ok(entity);
    }

    [HttpGet("{id}/items")]
    public async Task<Result<List<OutboundOrderItem>>> GetItems(int id)
    {
        var items = await _service.GetItemsByOrderId(id);
        return Result<List<OutboundOrderItem>>.Ok(items);
    }

    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateOutboundOrderInput input)
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
    public async Task<Result<bool>> Update([FromBody] UpdateOutboundOrderInput input)
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
            return result ? Result<bool>.Ok(true, "取消成功") : Result<bool>.Fail("取消失败");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }

    [HttpPost("{id}/confirm")]
    public async Task<Result<bool>> ConfirmOutbound(int id, [FromQuery] string? operatorName)
    {
        try
        {
            var result = await _service.ConfirmOutbound(id, operatorName);
            return result ? Result<bool>.Ok(true, "出库确认成功") : Result<bool>.Fail("出库确认失败");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
    }

    [HttpGet("flows")]
    public async Task<Result<PageResult<OutboundFlow>>> GetFlows(
        [FromQuery] string? orderNo,
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var (items, total) = await _service.GetFlowList(orderNo, keyword, pageIndex, pageSize);
        var pageResult = new PageResult<OutboundFlow>
        {
            Items = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Result<PageResult<OutboundFlow>>.Ok(pageResult);
    }
}
