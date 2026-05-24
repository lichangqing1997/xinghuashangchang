using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 销售单
/// </summary>
[SugarTable("SalesOrders")]
[SugarIndex("idx_sales_order_no", nameof(OrderNo), OrderByType.Desc)]
public class SalesOrder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? CustomerName { get; set; }

    /// <summary>
    /// 客户电话
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// 总金额
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 实收金额
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// 状态（Pending/Shipped/Completed/Returned/Cancelled）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "Pending";

    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? ShippedAt { get; set; }

    [SugarColumn(IsNullable = true)]
    public DateTime? CompletedAt { get; set; }
}
