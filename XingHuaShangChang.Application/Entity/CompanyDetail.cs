using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 公司扩展信息表
/// </summary>
[SugarTable("CompanyDetails")]
[SugarIndex("idx_company_detail_company_id", nameof(CompanyId), OrderByType.Desc)]
public class CompanyDetail
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 公司ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int CompanyId { get; set; }

    /// <summary>
    /// 字段名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 字段值
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? FieldValue { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
