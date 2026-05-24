using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// 创建菜单输入
/// </summary>
public class CreateMenuInput
{
    /// <summary>
    /// 父菜单ID（0表示顶级菜单）
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "父菜单ID无效")]
    public int ParentId { get; set; } = 0;

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "菜单名称长度为2-50个字符")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    [Required(ErrorMessage = "菜单编码不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "菜单编码长度为2-50个字符")]
    [RegularExpression(@"^[a-zA-Z_:]+$", ErrorMessage = "菜单编码只能包含英文字母、下划线和冒号")]
    public string MenuCode { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    [StringLength(200, ErrorMessage = "路由路径长度不能超过200个字符")]
    public string? Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(100, ErrorMessage = "图标长度不能超过100个字符")]
    public string? Icon { get; set; }

    /// <summary>
    /// 菜单类型：0=目录，1=菜单，2=按钮
    /// </summary>
    [Range(0, 2, ErrorMessage = "菜单类型值无效")]
    public int MenuType { get; set; } = 1;

    /// <summary>
    /// 权限标识
    /// </summary>
    [StringLength(100, ErrorMessage = "权限标识长度不能超过100个字符")]
    public string? Permission { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Range(0, 9999, ErrorMessage = "排序号范围为0-9999")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值无效")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否可见：0=隐藏，1=显示
    /// </summary>
    [Range(0, 1, ErrorMessage = "可见性值无效")]
    public int IsVisible { get; set; } = 1;
}

/// <summary>
/// 更新菜单输入
/// </summary>
public class UpdateMenuInput
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    [Required(ErrorMessage = "菜单ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "菜单ID无效")]
    public int Id { get; set; }

    /// <summary>
    /// 父菜单ID（0表示顶级菜单）
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "父菜单ID无效")]
    public int ParentId { get; set; } = 0;

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "菜单名称长度为2-50个字符")]
    public string MenuName { get; set; } = string.Empty;

    /// <summary>
    /// 路由路径
    /// </summary>
    [StringLength(200, ErrorMessage = "路由路径长度不能超过200个字符")]
    public string? Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(100, ErrorMessage = "图标长度不能超过100个字符")]
    public string? Icon { get; set; }

    /// <summary>
    /// 菜单类型：0=目录，1=菜单，2=按钮
    /// </summary>
    [Range(0, 2, ErrorMessage = "菜单类型值无效")]
    public int MenuType { get; set; } = 1;

    /// <summary>
    /// 权限标识
    /// </summary>
    [StringLength(100, ErrorMessage = "权限标识长度不能超过100个字符")]
    public string? Permission { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Range(0, 9999, ErrorMessage = "排序号范围为0-9999")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 状态：0=禁用，1=启用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值无效")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 是否可见：0=隐藏，1=显示
    /// </summary>
    [Range(0, 1, ErrorMessage = "可见性值无效")]
    public int IsVisible { get; set; } = 1;
}

/// <summary>
/// 菜单树输出
/// </summary>
public class MenuTreeOutput
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string MenuCode { get; set; } = string.Empty;
    public string? Path { get; set; }
    public string? Icon { get; set; }
    public int MenuType { get; set; }
    public string? Permission { get; set; }
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public int IsVisible { get; set; }
    public List<MenuTreeOutput> Children { get; set; } = new();
}
