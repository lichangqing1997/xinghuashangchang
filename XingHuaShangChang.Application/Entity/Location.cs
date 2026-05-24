using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 库位表
/// </summary>
[SugarTable("Locations")]
[SugarIndex("idx_location_code", nameof(LocationCode), OrderByType.Desc)]
public class Location
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(Length = 50, IsNullable = false)]
    public string LocationCode { get; set; } = string.Empty;

    [SugarColumn(Length = 100)]
    public string LocationName { get; set; } = string.Empty;

    /// <summary>
    /// 所属仓库
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Warehouse { get; set; }

    /// <summary>
    /// 货架
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Shelf { get; set; }

    /// <summary>
    /// 层
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? Floor { get; set; }

    /// <summary>
    /// 位
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? Position { get; set; }

    /// <summary>
    /// 库位状态（空闲/占用/锁定）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "空闲";

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
