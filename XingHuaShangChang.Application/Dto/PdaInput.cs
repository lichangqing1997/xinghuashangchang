using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// PDA扫码入库请求
/// </summary>
public class PdaScanInInput
{
    [Required(ErrorMessage = "条形码不能为空")]
    [StringLength(50, ErrorMessage = "条形码长度不能超过50个字符")]
    public string Barcode { get; set; } = string.Empty;

    [Required(ErrorMessage = "库位编码不能为空")]
    [StringLength(50, ErrorMessage = "库位编码长度不能超过50个字符")]
    public string LocationCode { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "数量必须在1-10000之间")]
    public int Quantity { get; set; } = 1;

    [StringLength(50, ErrorMessage = "进货单号长度不能超过50个字符")]
    public string? PurchaseOrderNo { get; set; }

    [StringLength(50, ErrorMessage = "操作人长度不能超过50个字符")]
    public string? Operator { get; set; }
}

/// <summary>
/// PDA扫码出库请求
/// </summary>
public class PdaScanOutInput
{
    [Required(ErrorMessage = "条形码不能为空")]
    [StringLength(50, ErrorMessage = "条形码长度不能超过50个字符")]
    public string Barcode { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "数量必须在1-10000之间")]
    public int Quantity { get; set; } = 1;

    [StringLength(50, ErrorMessage = "销售单号长度不能超过50个字符")]
    public string? SalesOrderNo { get; set; }

    [StringLength(50, ErrorMessage = "操作人长度不能超过50个字符")]
    public string? Operator { get; set; }
}

/// <summary>
/// PDA上架请求
/// </summary>
public class PdaShelfInput
{
    [Required(ErrorMessage = "条形码不能为空")]
    [StringLength(50, ErrorMessage = "条形码长度不能超过50个字符")]
    public string Barcode { get; set; } = string.Empty;

    [Required(ErrorMessage = "库位编码不能为空")]
    [StringLength(50, ErrorMessage = "库位编码长度不能超过50个字符")]
    public string LocationCode { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "数量必须在1-10000之间")]
    public int Quantity { get; set; } = 1;

    [StringLength(50, ErrorMessage = "操作人长度不能超过50个字符")]
    public string? Operator { get; set; }
}

/// <summary>
/// 手机端收货请求
/// </summary>
public class MobileReceiveInput
{
    [Required(ErrorMessage = "进货单号不能为空")]
    [StringLength(50, ErrorMessage = "进货单号长度不能超过50个字符")]
    public string PurchaseOrderNo { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "条形码长度不能超过50个字符")]
    public string? Barcode { get; set; }

    [Range(1, 10000, ErrorMessage = "数量必须在1-10000之间")]
    public int Quantity { get; set; } = 1;

    [StringLength(50, ErrorMessage = "操作人长度不能超过50个字符")]
    public string? Operator { get; set; }
}
