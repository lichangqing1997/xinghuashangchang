using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// 创建公司请求
/// </summary>
public class CreateCompanyInput
{
    [Required(ErrorMessage = "公司名称不能为空")]
    [StringLength(200, ErrorMessage = "公司名称长度不能超过200个字符")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "法人长度不能超过50个字符")]
    public string? LegalPerson { get; set; }

    [StringLength(100, ErrorMessage = "营业执照号长度不能超过100个字符")]
    public string? BusinessLicenseNo { get; set; }

    [StringLength(100, ErrorMessage = "公司单位编码长度不能超过100个字符")]
    public string? CompanyCode { get; set; }

    /// <summary>
    /// 扩展信息
    /// </summary>
    public List<CompanyDetailInput>? Details { get; set; }
}

/// <summary>
/// 更新公司请求
/// </summary>
public class UpdateCompanyInput
{
    [Required(ErrorMessage = "公司ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "公司ID无效")]
    public int Id { get; set; }

    [Required(ErrorMessage = "公司名称不能为空")]
    [StringLength(200, ErrorMessage = "公司名称长度不能超过200个字符")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "法人长度不能超过50个字符")]
    public string? LegalPerson { get; set; }

    [StringLength(100, ErrorMessage = "营业执照号长度不能超过100个字符")]
    public string? BusinessLicenseNo { get; set; }

    [StringLength(100, ErrorMessage = "公司单位编码长度不能超过100个字符")]
    public string? CompanyCode { get; set; }

    public int Status { get; set; } = 1;

    /// <summary>
    /// 扩展信息（替换全部）
    /// </summary>
    public List<CompanyDetailInput>? Details { get; set; }
}

/// <summary>
/// 公司扩展信息
/// </summary>
public class CompanyDetailInput
{
    [Required(ErrorMessage = "字段名不能为空")]
    [StringLength(100, ErrorMessage = "字段名长度不能超过100个字符")]
    public string FieldName { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "字段值长度不能超过500个字符")]
    public string? FieldValue { get; set; }
}
