using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 库存表
/// </summary>
[SugarTable("Inventory")]
[SugarIndex("idx_inventory_product_location", nameof(ProductId), OrderByType.Desc, nameof(LocationId), OrderByType.Desc)]
public class Inventory
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int ProductId { get; set; }

    /// <summary>
    /// 库位ID
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int LocationId { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Quantity { get; set; }

    /// <summary>
    /// 锁定数量（待出库）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int LockedQuantity { get; set; }

    /// <summary>
    /// 可用数量（计算属性，不映射到数据库）
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public int AvailableQuantity => Quantity - LockedQuantity;

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
