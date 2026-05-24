using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// 创建出库单请求
/// </summary>
public class CreateOutboundOrderInput
{
    [StringLength(200, ErrorMessage = "公司名长度不能超过200个字符")]
    public string? CompanyName { get; set; }

    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    [StringLength(50, ErrorMessage = "操作人长度不能超过50个字符")]
    public string? Operator { get; set; }

    [Required(ErrorMessage = "出库明细不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一行出库明细")]
    public List<CreateOutboundOrderItemInput> Items { get; set; } = new();
}

/// <summary>
/// 创建出库单明细请求
/// </summary>
public class CreateOutboundOrderItemInput
{
    [Required(ErrorMessage = "商品ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "商品ID无效")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "销售单价不能为空")]
    [Range(0, 999999.99, ErrorMessage = "销售单价范围为0-999999.99")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "出库数量不能为空")]
    [Range(1, 10000, ErrorMessage = "出库数量必须在1-10000之间")]
    public int Quantity { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public DateTime? ProductionDate { get; set; }

    [StringLength(100, ErrorMessage = "批号长度不能超过100个字符")]
    public string? BatchNo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "库位ID无效")]
    public int? LocationId { get; set; }

    [StringLength(200, ErrorMessage = "公司名长度不能超过200个字符")]
    public string? CompanyName { get; set; }
}

/// <summary>
/// 更新出库单请求
/// </summary>
public class UpdateOutboundOrderInput
{
    [Required(ErrorMessage = "出库单ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "出库单ID无效")]
    public int Id { get; set; }

    [StringLength(200, ErrorMessage = "公司名长度不能超过200个字符")]
    public string? CompanyName { get; set; }

    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    [StringLength(50, ErrorMessage = "操作人长度不能超过50个字符")]
    public string? Operator { get; set; }

    [Required(ErrorMessage = "出库明细不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一行出库明细")]
    public List<CreateOutboundOrderItemInput> Items { get; set; } = new();
}

/// <summary>
/// 出库单查询请求
/// </summary>
public class OutboundOrderQueryInput
{
    public string? Keyword { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
