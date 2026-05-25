using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// 创建租户请求
/// </summary>
public class CreateTenantInput
{
    [Required(ErrorMessage = "租户编码不能为空")]
    [StringLength(50, ErrorMessage = "租户编码长度不能超过50个字符")]
    public string TenantCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "租户名称不能为空")]
    [StringLength(200, ErrorMessage = "租户名称长度不能超过200个字符")]
    public string TenantName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "联系人长度不能超过50个字符")]
    public string? ContactPerson { get; set; }

    [StringLength(50, ErrorMessage = "联系电话长度不能超过50个字符")]
    public string? ContactPhone { get; set; }

    [StringLength(100, ErrorMessage = "联系邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? ContactEmail { get; set; }

    [StringLength(500, ErrorMessage = "地址长度不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(500, ErrorMessage = "连接字符串长度不能超过500个字符")]
    public string? ConnectionString { get; set; }

    [StringLength(100, ErrorMessage = "数据库名称长度不能超过100个字符")]
    public string? DatabaseName { get; set; }

    [Range(1, 10000, ErrorMessage = "最大用户数范围为1-10000")]
    public int MaxUsers { get; set; } = 100;

    public DateTime? ExpireAt { get; set; }

    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}

/// <summary>
/// 更新租户请求
/// </summary>
public class UpdateTenantInput
{
    [Required(ErrorMessage = "租户ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "租户ID无效")]
    public int Id { get; set; }

    [Required(ErrorMessage = "租户名称不能为空")]
    [StringLength(200, ErrorMessage = "租户名称长度不能超过200个字符")]
    public string TenantName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "联系人长度不能超过50个字符")]
    public string? ContactPerson { get; set; }

    [StringLength(50, ErrorMessage = "联系电话长度不能超过50个字符")]
    public string? ContactPhone { get; set; }

    [StringLength(100, ErrorMessage = "联系邮箱长度不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? ContactEmail { get; set; }

    [StringLength(500, ErrorMessage = "地址长度不能超过500个字符")]
    public string? Address { get; set; }

    [StringLength(500, ErrorMessage = "连接字符串长度不能超过500个字符")]
    public string? ConnectionString { get; set; }

    [StringLength(100, ErrorMessage = "数据库名称长度不能超过100个字符")]
    public string? DatabaseName { get; set; }

    [Range(1, 10000, ErrorMessage = "最大用户数范围为1-10000")]
    public int MaxUsers { get; set; } = 100;

    public DateTime? ExpireAt { get; set; }

    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    public int Status { get; set; } = 1;
}
