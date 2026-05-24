using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 进货单
/// </summary>
[SugarTable("PurchaseOrders")]
[SugarIndex("idx_purchase_order_no", nameof(OrderNo), OrderByType.Desc)]
public class PurchaseOrder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    [SugarColumn(IsNullable = false)]
    public int SupplierId { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 状态（Pending/Received/Stocked/Cancelled）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "Pending";

    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? ReceivedAt { get; set; }

    [SugarColumn(IsNullable = true)]
    public DateTime? StockedAt { get; set; }
}
