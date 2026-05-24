using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.RoleService;

/// <summary>
/// 角色服务实现
/// </summary>
public class RoleService : IRoleService
{
    private readonly ISqlSugarClient _db;

    public RoleService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<RoleDetailOutput> Items, int Total)> GetList(string? keyword = null, int status = -1, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<Role>();

        // 关键字搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(r => r.RoleCode.Contains(keyword) || r.RoleName.Contains(keyword));
        }

        // 状态筛选
        if (status >= 0)
        {
            query = query.Where(r => r.Status == status);
        }

        // 获取总数
        var total = await query.CountAsync();

        // 分页查询
        var roles = await query
            .OrderBy(r => r.SortOrder)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 批量获取角色菜单
        var roleIds = roles.Select(r => r.Id).ToList();
        var roleMenus = await _db.Queryable<RoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .ToListAsync();

        // 组装输出
        var items = roles.Select(r =>
        {
            var menuIds = roleMenus.Where(rm => rm.RoleId == r.Id).Select(rm => rm.MenuId).ToList();

            return new RoleDetailOutput
            {
                Id = r.Id,
                RoleCode = r.RoleCode,
                RoleName = r.RoleName,
                SortOrder = r.SortOrder,
                Status = r.Status,
                Remark = r.Remark,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                MenuIds = menuIds
            };
        }).ToList();

        return (items, total);
    }

    public async Task<List<Role>> GetAllActive()
    {
        return await _db.Queryable<Role>()
            .Where(r => r.Status == 1)
            .OrderBy(r => r.SortOrder)
            .ToListAsync();
    }

    public async Task<RoleDetailOutput?> GetById(int id)
    {
        var role = await _db.Queryable<Role>().InSingleAsync(id);
        if (role == null) return null;

        // 获取角色菜单
        var menuIds = await _db.Queryable<RoleMenu>()
            .Where(rm => rm.RoleId == id)
            .Select(rm => rm.MenuId)
            .ToListAsync();

        return new RoleDetailOutput
        {
            Id = role.Id,
            RoleCode = role.RoleCode,
            RoleName = role.RoleName,
            SortOrder = role.SortOrder,
            Status = role.Status,
            Remark = role.Remark,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            MenuIds = menuIds
        };
    }

    public async Task<int> Create(CreateRoleInput input)
    {
        // 检查角色编码是否已存在
        var exists = await _db.Queryable<Role>()
            .Where(r => r.RoleCode == input.RoleCode)
            .AnyAsync();
        if (exists)
            throw new InvalidOperationException("角色编码已存在");

        var role = new Role
        {
            RoleCode = input.RoleCode,
            RoleName = input.RoleName,
            SortOrder = input.SortOrder,
            Status = input.Status,
            Remark = input.Remark,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Ado.BeginTranAsync();
        try
        {
            var roleId = await _db.Insertable(role).ExecuteReturnIdentityAsync();

            // 分配菜单
            if (input.MenuIds.Any())
            {
                var roleMenus = input.MenuIds.Select(menuId => new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _db.Insertable(roleMenus).ExecuteCommandAsync();
            }

            await _db.Ado.CommitTranAsync();
            return roleId;
        }
        catch
        {
            await _db.Ado.RollbackTranAsync();
            throw;
        }
    }

    public async Task<bool> Update(UpdateRoleInput input)
    {
        var role = await _db.Queryable<Role>().InSingleAsync(input.Id);
        if (role == null)
            throw new InvalidOperationException("角色不存在");

        // 更新角色信息
        role.RoleName = input.RoleName;
        role.SortOrder = input.SortOrder;
        role.Status = input.Status;
        role.Remark = input.Remark;
        role.UpdatedAt = DateTime.UtcNow;

        await _db.Ado.BeginTranAsync();
        try
        {
            await _db.Updateable(role).ExecuteCommandHasChangeAsync();

            // 更新菜单关联
            await _db.Deleteable<RoleMenu>()
                .Where(rm => rm.RoleId == input.Id)
                .ExecuteCommandAsync();

            if (input.MenuIds.Any())
            {
                var roleMenus = input.MenuIds.Select(menuId => new RoleMenu
                {
                    RoleId = input.Id,
                    MenuId = menuId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _db.Insertable(roleMenus).ExecuteCommandAsync();
            }

            await _db.Ado.CommitTranAsync();
            return true;
        }
        catch
        {
            await _db.Ado.RollbackTranAsync();
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        var role = await _db.Queryable<Role>().InSingleAsync(id);
        if (role == null)
            throw new InvalidOperationException("角色不存在");

        // 检查是否有用户使用此角色
        var hasUsers = await _db.Queryable<UserRole>()
            .Where(ur => ur.RoleId == id)
            .AnyAsync();
        if (hasUsers)
            throw new InvalidOperationException("该角色下存在用户，无法删除");

        await _db.Ado.BeginTranAsync();
        try
        {
            // 删除角色菜单关联
            await _db.Deleteable<RoleMenu>()
                .Where(rm => rm.RoleId == id)
                .ExecuteCommandAsync();

            // 删除角色
            await _db.Deleteable<Role>()
                .Where(r => r.Id == id)
                .ExecuteCommandAsync();

            await _db.Ado.CommitTranAsync();
            return true;
        }
        catch
        {
            await _db.Ado.RollbackTranAsync();
            throw;
        }
    }
}
