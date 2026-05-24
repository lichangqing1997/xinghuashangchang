using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 公司表
/// </summary>
[SugarTable("Companies")]
[SugarIndex("idx_company_name", nameof(CompanyName), OrderByType.Desc)]
public class Company
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 法人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LegalPerson { get; set; }

    /// <summary>
    /// 营业执照号
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? BusinessLicenseNo { get; set; }

    /// <summary>
    /// 公司单位编码
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? CompanyCode { get; set; }

    /// <summary>
    /// 状态（0-禁用 1-启用）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Status { get; set; } = 1;

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
