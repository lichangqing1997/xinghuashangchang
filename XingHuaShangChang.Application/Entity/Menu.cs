using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 菜单表
/// </summary>
[SugarTable("Menus")]
public class Menu
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 父菜单ID（0表示顶级菜单）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ParentId { get; set; } = 0;

    /// <summary>
    /// 菜单名称
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string MenuCode { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Icon { get; set; }

    /// <summary>
    /// 菜单类型：0=目录，1=菜单，2=按钮
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int MenuType { get; set; } = 1;

    /// <summary>
    /// 权限标识
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Permission { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否可见：0=隐藏，1=显示
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int IsVisible { get; set; } = 1;

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
    /// 子菜单（非数据库字段）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<Menu> Children { get; set; } = new();
}
