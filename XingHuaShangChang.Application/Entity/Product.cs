using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 商品表
/// </summary>
[SugarTable("Products")]
[SugarIndex("idx_product_barcode", nameof(Barcode), OrderByType.Desc)]
[SugarIndex("idx_product_code", nameof(ProductCode), OrderByType.Desc)]
public class Product
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 商品编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 条形码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// SKU
    /// </summary>
    [SugarColumn(Length = 100)]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// 商品类别（衣服、鞋子等）
    /// </summary>
    [SugarColumn(Length = 50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 颜色
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Color { get; set; }

    /// <summary>
    /// 尺码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Size { get; set; }

    /// <summary>
    /// 规格
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Specification { get; set; }

    /// <summary>
    /// 生产厂家
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 注册证号
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? RegistrationNo { get; set; }

    /// <summary>
    /// 进货价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal PurchasePrice { get; set; }

    /// <summary>
    /// 销售价
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public decimal SalePrice { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Unit { get; set; } = "件";

    /// <summary>
    /// 供应商ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? SupplierId { get; set; }

    /// <summary>
    /// 状态（0-禁用 1-启用）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Status { get; set; } = 1;

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
