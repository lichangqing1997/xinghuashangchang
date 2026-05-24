using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.UserService;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    Task<(List<UserDetailOutput> Items, int Total)> GetList(string? keyword = null, int status = -1, int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 根据ID获取用户详情
    /// </summary>
    Task<UserDetailOutput?> GetById(int id);

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    Task<User?> GetByUsername(string username);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<int> Create(CreateUserInput input);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<bool> Update(UpdateUserInput input);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<bool> Delete(int id);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task<bool> ChangePassword(ChangePasswordInput input);

    /// <summary>
    /// 验证密码
    /// </summary>
    Task<bool> ValidatePassword(string username, string password);

    /// <summary>
    /// 记录登录时间
    /// </summary>
    Task RecordLoginTime(int userId);

    /// <summary>
    /// 更新用户头像
    /// </summary>
    Task<bool> UpdateAvatar(int userId, string avatarUrl);
}
