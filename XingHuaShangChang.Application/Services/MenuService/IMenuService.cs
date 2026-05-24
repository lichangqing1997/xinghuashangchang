using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.MenuService;

/// <summary>
/// 菜单服务接口
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// 获取菜单列表（平铺）
    /// </summary>
    Task<List<Menu>> GetList(string? keyword = null, int status = -1);

    /// <summary>
    /// 获取菜单树
    /// </summary>
    Task<List<MenuTreeOutput>> GetTree(string? keyword = null, int status = -1);

    /// <summary>
    /// 根据用户ID获取菜单树
    /// </summary>
    Task<List<MenuTreeOutput>> GetTreeByUserId(int userId);

    /// <summary>
    /// 根据ID获取菜单
    /// </summary>
    Task<Menu?> GetById(int id);

    /// <summary>
    /// 创建菜单
    /// </summary>
    Task<int> Create(CreateMenuInput input);

    /// <summary>
    /// 更新菜单
    /// </summary>
    Task<bool> Update(UpdateMenuInput input);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task<bool> Delete(int id);
}
