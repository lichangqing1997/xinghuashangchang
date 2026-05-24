using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// 创建角色输入
/// </summary>
public class CreateRoleInput
{
    /// <summary>
    /// 角色编码
    /// </summary>
    [Required(ErrorMessage = "角色编码不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "角色编码长度为2-50个字符")]
    [RegularExpression(@"^[a-zA-Z_]+$", ErrorMessage = "角色编码只能包含英文字母和下划线")]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "角色名称长度为2-50个字符")]
    public string RoleName { get; set; } = string.Empty;

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
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<int> MenuIds { get; set; } = new();
}

/// <summary>
/// 更新角色输入
/// </summary>
public class UpdateRoleInput
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Required(ErrorMessage = "角色ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "角色ID无效")]
    public int Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "角色名称长度为2-50个字符")]
    public string RoleName { get; set; } = string.Empty;

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
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<int> MenuIds { get; set; } = new();
}

/// <summary>
/// 角色详情输出（包含菜单信息）
/// </summary>
public class RoleDetailOutput
{
    public int Id { get; set; }
    public string RoleCode { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<int> MenuIds { get; set; } = new();
}
