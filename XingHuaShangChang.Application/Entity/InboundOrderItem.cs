using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 入库单明细
/// </summary>
[SugarTable("InboundOrderItems")]
[SugarIndex("idx_inbound_order_item_order_id", nameof(InboundOrderId), OrderByType.Desc)]
public class InboundOrderItem
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 入库单ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int InboundOrderId { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int SeqNo { get; set; }

    /// <summary>
    /// 物料编码
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? MaterialCode { get; set; }

    /// <summary>
    /// 物料名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? MaterialName { get; set; }

    /// <summary>
    /// 采购数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int PurchaseQuantity { get; set; }

    /// <summary>
    /// 批次号
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? BatchNo { get; set; }

    /// <summary>
    /// 批次1
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch1 { get; set; }

    /// <summary>
    /// 批次2
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch2 { get; set; }

    /// <summary>
    /// 批次3
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch3 { get; set; }

    /// <summary>
    /// 批次4
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch4 { get; set; }

    /// <summary>
    /// 批次5
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch5 { get; set; }

    /// <summary>
    /// 批次6
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch6 { get; set; }

    /// <summary>
    /// 批次7
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch7 { get; set; }

    /// <summary>
    /// 批次8
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch8 { get; set; }

    /// <summary>
    /// 批次9
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch9 { get; set; }

    /// <summary>
    /// 批次10
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Batch10 { get; set; }

    /// <summary>
    /// 预留备注1
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark1 { get; set; }

    /// <summary>
    /// 预留备注2
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark2 { get; set; }

    /// <summary>
    /// 预留备注3
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark3 { get; set; }

    /// <summary>
    /// 预留备注4
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark4 { get; set; }

    /// <summary>
    /// 预留备注5
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark5 { get; set; }

    /// <summary>
    /// 预留备注6
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark6 { get; set; }

    /// <summary>
    /// 预留备注7
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark7 { get; set; }

    /// <summary>
    /// 预留备注8
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark8 { get; set; }

    /// <summary>
    /// 预留备注9
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark9 { get; set; }

    /// <summary>
    /// 预留备注10
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ReserveRemark10 { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
