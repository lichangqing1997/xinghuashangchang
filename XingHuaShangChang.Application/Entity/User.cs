using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("Users")]
[SugarIndex("idx_user_username", nameof(Username), OrderByType.Desc)]
[SugarIndex("idx_user_phone", nameof(Phone), OrderByType.Desc)]
public class User
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（BCrypt加密）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Email { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? LastLoginAt { get; set; }
}
