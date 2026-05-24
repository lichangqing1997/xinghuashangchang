using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 销售明细
/// </summary>
[SugarTable("SalesOrderItems")]
[SugarIndex("idx_sales_order_item_order_id", nameof(SalesOrderId), OrderByType.Desc)]
public class SalesOrderItem
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(IsNullable = false)]
    public int SalesOrderId { get; set; }

    [SugarColumn(IsNullable = false)]
    public int ProductId { get; set; }

    /// <summary>
    /// 销售数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 销售单价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 小计金额（计算属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public decimal Amount => Quantity * UnitPrice;

    /// <summary>
    /// 已出库数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ShippedQuantity { get; set; }

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
