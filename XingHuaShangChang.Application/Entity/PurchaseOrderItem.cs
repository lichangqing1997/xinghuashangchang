using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 进货明细
/// </summary>
[SugarTable("PurchaseOrderItems")]
[SugarIndex("idx_purchase_order_item_order_id", nameof(PurchaseOrderId), OrderByType.Desc)]
public class PurchaseOrderItem
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(IsNullable = false)]
    public int PurchaseOrderId { get; set; }

    [SugarColumn(IsNullable = false)]
    public int ProductId { get; set; }

    /// <summary>
    /// 进货数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 进货单价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 小计金额（计算属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public decimal Amount => Quantity * UnitPrice;

    /// <summary>
    /// 已收货数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ReceivedQuantity { get; set; }

    /// <summary>
    /// 已入库数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int StockedQuantity { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
