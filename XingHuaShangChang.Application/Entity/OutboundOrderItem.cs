using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 出库单明细
/// </summary>
[SugarTable("OutboundOrderItems")]
[SugarIndex("idx_outbound_order_item_order_id", nameof(OutboundOrderId), OrderByType.Desc)]
public class OutboundOrderItem
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 出库单ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int OutboundOrderId { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ProductId { get; set; }

    /// <summary>
    /// 规格（快照）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Specification { get; set; }

    /// <summary>
    /// 销售单价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 出库数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 出库总价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal Amount { get; set; }

    /// <summary>
    /// 生产厂家（快照）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 有效期
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// 生产日期
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ProductionDate { get; set; }

    /// <summary>
    /// 批号
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? BatchNo { get; set; }

    /// <summary>
    /// 注册证号（快照）
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? RegistrationNo { get; set; }

    /// <summary>
    /// 库位ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? LocationId { get; set; }

    /// <summary>
    /// 公司名（行级覆盖）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? CompanyName { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
