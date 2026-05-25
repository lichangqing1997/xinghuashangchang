using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.TenantService;

public interface ITenantService
{
    Task<(List<Tenant> Items, int Total)> GetList(string? keyword, int pageIndex = 1, int pageSize = 20);
    Task<Tenant?> GetById(int id);
    Task<Tenant?> GetByCode(string tenantCode);
    Task<int> Create(CreateTenantInput input);
    Task<bool> Update(UpdateTenantInput input);
    Task<bool> Delete(int id);
}
