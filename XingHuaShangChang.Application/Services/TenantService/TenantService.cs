using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.TenantService;

public class TenantService : ITenantService
{
    private readonly ISqlSugarClient _db;

    public TenantService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<Tenant> Items, int Total)> GetList(string? keyword, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<Tenant>().Where(x => x.Status == 1);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.TenantName.Contains(keyword) || x.TenantCode.Contains(keyword) || (x.ContactPerson != null && x.ContactPerson.Contains(keyword)));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Tenant?> GetById(int id)
    {
        return await _db.Queryable<Tenant>().Where(x => x.Id == id && x.Status == 1).FirstAsync();
    }

    public async Task<Tenant?> GetByCode(string tenantCode)
    {
        return await _db.Queryable<Tenant>().Where(x => x.TenantCode == tenantCode && x.Status == 1).FirstAsync();
    }

    public async Task<int> Create(CreateTenantInput input)
    {
        // 检查租户编码是否已存在
        var existing = await GetByCode(input.TenantCode);
        if (existing != null)
            throw new InvalidOperationException("租户编码已存在");

        var tenant = new Tenant
        {
            TenantCode = input.TenantCode,
            TenantName = input.TenantName,
            ContactPerson = input.ContactPerson,
            ContactPhone = input.ContactPhone,
            ContactEmail = input.ContactEmail,
            Address = input.Address,
            ConnectionString = input.ConnectionString,
            DatabaseName = input.DatabaseName,
            MaxUsers = input.MaxUsers,
            ExpireAt = input.ExpireAt,
            Remark = input.Remark
        };

        return await _db.Insertable(tenant).ExecuteReturnIdentityAsync();
    }

    public async Task<bool> Update(UpdateTenantInput input)
    {
        var tenant = await _db.Queryable<Tenant>().Where(x => x.Id == input.Id && x.Status == 1).FirstAsync();
        if (tenant == null)
            throw new InvalidOperationException("租户不存在");

        tenant.TenantName = input.TenantName;
        tenant.ContactPerson = input.ContactPerson;
        tenant.ContactPhone = input.ContactPhone;
        tenant.ContactEmail = input.ContactEmail;
        tenant.Address = input.Address;
        tenant.ConnectionString = input.ConnectionString;
        tenant.DatabaseName = input.DatabaseName;
        tenant.MaxUsers = input.MaxUsers;
        tenant.ExpireAt = input.ExpireAt;
        tenant.Remark = input.Remark;
        tenant.Status = input.Status;
        tenant.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(tenant).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var tenant = await _db.Queryable<Tenant>().Where(x => x.Id == id && x.Status == 1).FirstAsync();
        if (tenant == null)
            throw new InvalidOperationException("租户不存在");

        tenant.Status = 0;
        tenant.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(tenant).UpdateColumns(x => new { x.Status, x.UpdatedAt }).ExecuteCommandHasChangeAsync();
    }
}
