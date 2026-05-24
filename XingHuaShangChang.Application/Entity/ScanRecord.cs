using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// PDA扫码记录
/// </summary>
[SugarTable("ScanRecords")]
[SugarIndex("idx_scan_record_barcode", nameof(Barcode), OrderByType.Desc)]
[SugarIndex("idx_scan_record_scan_time", nameof(ScanTime), OrderByType.Desc)]
public class ScanRecord
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 条形码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ProductId { get; set; }

    /// <summary>
    /// 操作类型（In/Out/Shelf）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 库位ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? LocationId { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 关联单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ReferenceNo { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Operator { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime ScanTime { get; set; } = DateTime.UtcNow;
}
