using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 供应商表
/// </summary>
[SugarTable("Suppliers")]
[SugarIndex("idx_supplier_code", nameof(SupplierCode), OrderByType.Desc)]
public class Supplier
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(Length = 100, IsNullable = false)]
    public string SupplierCode { get; set; } = string.Empty;

    [SugarColumn(Length = 200, IsNullable = false)]
    public string SupplierName { get; set; } = string.Empty;

    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ContactPerson { get; set; }

    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ContactPhone { get; set; }

    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Address { get; set; }

    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(IsNullable = false)]
    public int Status { get; set; } = 1;

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
