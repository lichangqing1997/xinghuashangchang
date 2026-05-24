using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 角色菜单关联表
/// </summary>
[SugarTable("RoleMenus")]
[SugarIndex("idx_rolemenu_role", nameof(RoleId), OrderByType.Desc)]
[SugarIndex("idx_rolemenu_menu", nameof(MenuId), OrderByType.Desc)]
public class RoleMenu
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int RoleId { get; set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int MenuId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
