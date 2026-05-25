using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.InboundOrderService;

public interface IInboundOrderService
{
    Task<(List<InboundOrder> Items, int Total)> GetList(InboundOrderQueryInput query);
    Task<InboundOrder?> GetById(int id);
    Task<List<InboundOrderItem>> GetItemsByOrderId(int orderId);
    Task<int> Create(CreateInboundOrderInput input);
    Task<bool> Update(UpdateInboundOrderInput input);
    Task<bool> Delete(int id);
    Task<bool> Audit(int orderId, AuditInboundOrderInput input);
    Task<bool> ChangeStatus(int orderId, string newStatus, string? operatorName);
}
