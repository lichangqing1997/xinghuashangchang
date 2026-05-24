using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.OutboundOrderService;

public interface IOutboundOrderService
{
    Task<(List<OutboundOrder> Items, int Total)> GetList(OutboundOrderQueryInput query);
    Task<OutboundOrder?> GetById(int id);
    Task<List<OutboundOrderItem>> GetItemsByOrderId(int orderId);
    Task<int> Create(CreateOutboundOrderInput input);
    Task<bool> Update(UpdateOutboundOrderInput input);
    Task<bool> Delete(int id);
    Task<bool> ConfirmOutbound(int orderId, string? operatorName);
    Task<(List<OutboundFlow> Items, int Total)> GetFlowList(string? orderNo, string? keyword, int pageIndex = 1, int pageSize = 20);
}
