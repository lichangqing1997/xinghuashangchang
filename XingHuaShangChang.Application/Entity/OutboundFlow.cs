using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 出库流水
/// </summary>
[SugarTable("OutboundFlows")]
[SugarIndex("idx_outbound_flow_order_no", nameof(OrderNo), OrderByType.Desc)]
[SugarIndex("idx_outbound_flow_time", nameof(FlowTime), OrderByType.Desc)]
public class OutboundFlow
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 出库单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ProductId { get; set; }

    /// <summary>
    /// 商品名称（冗余）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? ProductName { get; set; }

    /// <summary>
    /// 商品编码（冗余）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ProductCode { get; set; }

    /// <summary>
    /// 库位ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int LocationId { get; set; }

    /// <summary>
    /// 库位编码（冗余）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? LocationCode { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 批号
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? BatchNo { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Operator { get; set; }

    /// <summary>
    /// 出库时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime FlowTime { get; set; } = DateTime.UtcNow;
}
