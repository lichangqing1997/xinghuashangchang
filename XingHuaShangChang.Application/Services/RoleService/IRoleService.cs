using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.RoleService;

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 获取角色列表（分页）
    /// </summary>
    Task<(List<RoleDetailOutput> Items, int Total)> GetList(string? keyword = null, int status = -1, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 获取所有启用的角色
    /// </summary>
    Task<List<Role>> GetAllActive();

    /// <summary>
    /// 根据ID获取角色详情
    /// </summary>
    Task<RoleDetailOutput?> GetById(int id);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<int> Create(CreateRoleInput input);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task<bool> Update(UpdateRoleInput input);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task<bool> Delete(int id);
}
