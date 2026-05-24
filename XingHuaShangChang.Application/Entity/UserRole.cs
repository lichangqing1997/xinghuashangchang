using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 用户角色关联表
/// </summary>
[SugarTable("UserRoles")]
[SugarIndex("idx_userrole_user", nameof(UserId), OrderByType.Desc)]
[SugarIndex("idx_userrole_role", nameof(RoleId), OrderByType.Desc)]
public class UserRole
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int RoleId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
