using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 出库单
/// </summary>
[SugarTable("OutboundOrders")]
[SugarIndex("idx_outbound_order_no", nameof(OrderNo), OrderByType.Desc)]
public class OutboundOrder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 出库单号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 收货公司名
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 状态（待出库/已出库/已取消）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "待出库";

    /// <summary>
    /// 制单人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Operator { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 出库确认时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? OutboundAt { get; set; }

    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
