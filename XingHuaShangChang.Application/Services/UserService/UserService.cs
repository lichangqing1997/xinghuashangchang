using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.UserService;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly ISqlSugarClient _db;

    public UserService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<UserDetailOutput> Items, int Total)> GetList(string? keyword = null, int status = -1, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<User>();

        // 关键字搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u => u.Username.Contains(keyword) ||
                                     (u.RealName != null && u.RealName.Contains(keyword)) ||
                                     (u.Phone != null && u.Phone.Contains(keyword)));
        }

        // 状态筛选
        if (status >= 0)
        {
            query = query.Where(u => u.Status == status);
        }

        // 获取总数
        var total = await query.CountAsync();

        // 分页查询
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 批量获取用户角色
        var userIds = users.Select(u => u.Id).ToList();
        var userRoles = await _db.Queryable<UserRole>()
            .Where(ur => userIds.Contains(ur.UserId))
            .ToListAsync();
        var roleIds = userRoles.Select(ur => ur.RoleId).Distinct().ToList();
        var roles = await _db.Queryable<Role>()
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();

        // 组装输出
        var items = users.Select(u =>
        {
            var userRoleIds = userRoles.Where(ur => ur.UserId == u.Id).Select(ur => ur.RoleId).ToList();
            var userRolesList = roles.Where(r => userRoleIds.Contains(r.Id))
                .Select(r => new RoleOutput { Id = r.Id, RoleCode = r.RoleCode, RoleName = r.RoleName })
                .ToList();

            return new UserDetailOutput
            {
                Id = u.Id,
                Username = u.Username,
                RealName = u.RealName,
                Phone = u.Phone,
                Email = u.Email,
                Avatar = u.Avatar,
                Status = u.Status,
                Remark = u.Remark,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                LastLoginAt = u.LastLoginAt,
                Roles = userRolesList,
                RoleIds = userRoleIds
            };
        }).ToList();

        return (items, total);
    }

    public async Task<UserDetailOutput?> GetById(int id)
    {
        var user = await _db.Queryable<User>().InSingleAsync(id);
        if (user == null) return null;

        // 获取用户角色
        var userRoles = await _db.Queryable<UserRole>()
            .Where(ur => ur.UserId == id)
            .ToListAsync();
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
        var roles = await _db.Queryable<Role>()
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();

        return new UserDetailOutput
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Phone = user.Phone,
            Email = user.Email,
            Avatar = user.Avatar,
            Status = user.Status,
            Remark = user.Remark,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.Select(r => new RoleOutput { Id = r.Id, RoleCode = r.RoleCode, RoleName = r.RoleName }).ToList(),
            RoleIds = roleIds
        };
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _db.Queryable<User>()
            .Where(u => u.Username == username)
            .FirstAsync();
    }

    public async Task<int> Create(CreateUserInput input)
    {
        // 检查用户名是否已存在
        var exists = await _db.Queryable<User>()
            .Where(u => u.Username == input.Username)
            .AnyAsync();
        if (exists)
            throw new InvalidOperationException("用户名已存在");

        // 检查手机号是否已存在
        if (!string.IsNullOrWhiteSpace(input.Phone))
        {
            var phoneExists = await _db.Queryable<User>()
                .Where(u => u.Phone == input.Phone)
                .AnyAsync();
            if (phoneExists)
                throw new InvalidOperationException("手机号已被使用");
        }

        // 创建用户
        var user = new User
        {
            Username = input.Username,
            Password = HashPassword(input.Password),
            RealName = input.RealName,
            Phone = input.Phone,
            Email = input.Email,
            Status = input.Status,
            Remark = input.Remark,
            CreatedAt = DateTime.UtcNow
        };

        // 使用事务确保数据一致性
        await _db.Ado.BeginTranAsync();
        try
        {
            var userId = await _db.Insertable(user).ExecuteReturnIdentityAsync();

            // 分配角色
            if (input.RoleIds.Any())
            {
                var userRoles = input.RoleIds.Select(roleId => new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _db.Insertable(userRoles).ExecuteCommandAsync();
            }

            await _db.Ado.CommitTranAsync();
            return userId;
        }
        catch
        {
            await _db.Ado.RollbackTranAsync();
            throw;
        }
    }

    public async Task<bool> Update(UpdateUserInput input)
    {
        var user = await _db.Queryable<User>().InSingleAsync(input.Id);
        if (user == null)
            throw new InvalidOperationException("用户不存在");

        // 检查手机号是否已被其他用户使用
        if (!string.IsNullOrWhiteSpace(input.Phone))
        {
            var phoneExists = await _db.Queryable<User>()
                .Where(u => u.Phone == input.Phone && u.Id != input.Id)
                .AnyAsync();
            if (phoneExists)
                throw new InvalidOperationException("手机号已被其他用户使用");
        }

        // 更新用户信息
        user.RealName = input.RealName;
        user.Phone = input.Phone;
        user.Email = input.Email;
        user.Status = input.Status;
        user.Remark = input.Remark;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.Ado.BeginTranAsync();
        try
        {
            await _db.Updateable(user).ExecuteCommandHasChangeAsync();

            // 更新角色关联
            await _db.Deleteable<UserRole>()
                .Where(ur => ur.UserId == input.Id)
                .ExecuteCommandAsync();

            if (input.RoleIds.Any())
            {
                var userRoles = input.RoleIds.Select(roleId => new UserRole
                {
                    UserId = input.Id,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _db.Insertable(userRoles).ExecuteCommandAsync();
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
        var user = await _db.Queryable<User>().InSingleAsync(id);
        if (user == null)
            throw new InvalidOperationException("用户不存在");

        // 不允许删除admin用户
        if (user.Username == "admin")
            throw new InvalidOperationException("不能删除管理员账户");

        await _db.Ado.BeginTranAsync();
        try
        {
            // 删除用户角色关联
            await _db.Deleteable<UserRole>()
                .Where(ur => ur.UserId == id)
                .ExecuteCommandAsync();

            // 删除用户
            await _db.Deleteable<User>()
                .Where(u => u.Id == id)
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

    public async Task<bool> ChangePassword(ChangePasswordInput input)
    {
        var user = await _db.Queryable<User>().InSingleAsync(input.UserId);
        if (user == null)
            throw new InvalidOperationException("用户不存在");

        // 验证原密码
        if (!VerifyPassword(input.OldPassword, user.Password))
            throw new InvalidOperationException("原密码不正确");

        user.Password = HashPassword(input.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(user).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> ValidatePassword(string username, string password)
    {
        var user = await GetByUsername(username);
        if (user == null || user.Status != 1)
            return false;

        return VerifyPassword(password, user.Password);
    }

    public async Task RecordLoginTime(int userId)
    {
        await _db.Updateable<User>()
            .SetColumns(u => u.LastLoginAt == DateTime.UtcNow)
            .Where(u => u.Id == userId)
            .ExecuteCommandAsync();
    }

    public async Task<bool> UpdateAvatar(int userId, string avatarUrl)
    {
        var user = await _db.Queryable<User>().InSingleAsync(userId);
        if (user == null)
            throw new InvalidOperationException("用户不存在");

        user.Avatar = avatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(user).ExecuteCommandHasChangeAsync();
    }

    /// <summary>
    /// 密码哈希（使用BCrypt）
    /// </summary>
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
