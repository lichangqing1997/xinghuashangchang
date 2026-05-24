using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.MenuService;

/// <summary>
/// 菜单服务实现
/// </summary>
public class MenuService : IMenuService
{
    private readonly ISqlSugarClient _db;

    public MenuService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<List<Menu>> GetList(string? keyword = null, int status = -1)
    {
        var query = _db.Queryable<Menu>();

        // 关键字搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(m => m.MenuName.Contains(keyword) || m.MenuCode.Contains(keyword));
        }

        // 状态筛选
        if (status >= 0)
        {
            query = query.Where(m => m.Status == status);
        }

        return await query
            .OrderBy(m => m.SortOrder)
            .ToListAsync();
    }

    public async Task<List<MenuTreeOutput>> GetTree(string? keyword = null, int status = -1)
    {
        var menus = await GetList(keyword, status);
        return BuildTree(menus, 0);
    }

    public async Task<List<MenuTreeOutput>> GetTreeByUserId(int userId)
    {
        // 获取用户角色
        var roleIds = await _db.Queryable<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (!roleIds.Any())
            return new List<MenuTreeOutput>();

        // 获取角色关联的菜单ID
        var menuIds = await _db.Queryable<RoleMenu>()
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .Distinct()
            .ToListAsync();

        // 获取菜单列表
        var menus = await _db.Queryable<Menu>()
            .Where(m => menuIds.Contains(m.Id) && m.Status == 1)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        return BuildTree(menus, 0);
    }

    public async Task<Menu?> GetById(int id)
    {
        return await _db.Queryable<Menu>().InSingleAsync(id);
    }

    public async Task<int> Create(CreateMenuInput input)
    {
        // 检查菜单编码是否已存在
        var exists = await _db.Queryable<Menu>()
            .Where(m => m.MenuCode == input.MenuCode)
            .AnyAsync();
        if (exists)
            throw new InvalidOperationException("菜单编码已存在");

        // 如果有父菜单，检查父菜单是否存在
        if (input.ParentId > 0)
        {
            var parentMenu = await _db.Queryable<Menu>().InSingleAsync(input.ParentId);
            if (parentMenu == null)
                throw new InvalidOperationException("父菜单不存在");
        }

        var menu = new Menu
        {
            ParentId = input.ParentId,
            MenuName = input.MenuName,
            MenuCode = input.MenuCode,
            Path = input.Path,
            Icon = input.Icon,
            MenuType = input.MenuType,
            Permission = input.Permission,
            SortOrder = input.SortOrder,
            Status = input.Status,
            IsVisible = input.IsVisible,
            CreatedAt = DateTime.UtcNow
        };

        return await _db.Insertable(menu).ExecuteReturnIdentityAsync();
    }

    public async Task<bool> Update(UpdateMenuInput input)
    {
        var menu = await _db.Queryable<Menu>().InSingleAsync(input.Id);
        if (menu == null)
            throw new InvalidOperationException("菜单不存在");

        // 不能将自己设为自己的父菜单
        if (input.ParentId == input.Id)
            throw new InvalidOperationException("不能将菜单设为自己的子菜单");

        // 如果有父菜单，检查父菜单是否存在
        if (input.ParentId > 0)
        {
            var parentMenu = await _db.Queryable<Menu>().InSingleAsync(input.ParentId);
            if (parentMenu == null)
                throw new InvalidOperationException("父菜单不存在");
        }

        // 更新菜单信息
        menu.ParentId = input.ParentId;
        menu.MenuName = input.MenuName;
        menu.Path = input.Path;
        menu.Icon = input.Icon;
        menu.MenuType = input.MenuType;
        menu.Permission = input.Permission;
        menu.SortOrder = input.SortOrder;
        menu.Status = input.Status;
        menu.IsVisible = input.IsVisible;
        menu.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(menu).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var menu = await _db.Queryable<Menu>().InSingleAsync(id);
        if (menu == null)
            throw new InvalidOperationException("菜单不存在");

        // 检查是否有子菜单
        var hasChildren = await _db.Queryable<Menu>()
            .Where(m => m.ParentId == id)
            .AnyAsync();
        if (hasChildren)
            throw new InvalidOperationException("该菜单下存在子菜单，无法删除");

        // 检查是否有角色使用此菜单
        var hasRoles = await _db.Queryable<RoleMenu>()
            .Where(rm => rm.MenuId == id)
            .AnyAsync();
        if (hasRoles)
            throw new InvalidOperationException("该菜单已被角色使用，无法删除");

        return await _db.Deleteable<Menu>()
            .Where(m => m.Id == id)
            .ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    private List<MenuTreeOutput> BuildTree(List<Menu> menus, int parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.SortOrder)
            .Select(m => new MenuTreeOutput
            {
                Id = m.Id,
                ParentId = m.ParentId,
                MenuName = m.MenuName,
                MenuCode = m.MenuCode,
                Path = m.Path,
                Icon = m.Icon,
                MenuType = m.MenuType,
                Permission = m.Permission,
                SortOrder = m.SortOrder,
                Status = m.Status,
                IsVisible = m.IsVisible,
                Children = BuildTree(menus, m.Id)
            })
            .ToList();
    }
}
