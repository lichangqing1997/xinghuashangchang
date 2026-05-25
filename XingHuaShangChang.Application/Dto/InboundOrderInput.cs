using System.ComponentModel.DataAnnotations;

namespace XingHuaShangChang.Application.Dto;

/// <summary>
/// 创建入库单请求
/// </summary>
public class CreateInboundOrderInput
{
    [StringLength(200, ErrorMessage = "供应商名称长度不能超过200个字符")]
    public string? SupplierName { get; set; }

    [StringLength(200, ErrorMessage = "来源长度不能超过200个字符")]
    public string? Source { get; set; }

    [StringLength(50, ErrorMessage = "入库单类型长度不能超过50个字符")]
    public string? OrderType { get; set; }

    [StringLength(50, ErrorMessage = "创建人长度不能超过50个字符")]
    public string? Creator { get; set; }

    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    [Required(ErrorMessage = "入库明细不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一行入库明细")]
    public List<CreateInboundOrderItemInput> Items { get; set; } = new();
}

/// <summary>
/// 创建入库单明细请求
/// </summary>
public class CreateInboundOrderItemInput
{
    [Required(ErrorMessage = "序号不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "序号必须大于0")]
    public int SeqNo { get; set; }

    [StringLength(100, ErrorMessage = "物料编码长度不能超过100个字符")]
    public string? MaterialCode { get; set; }

    [StringLength(200, ErrorMessage = "物料名称长度不能超过200个字符")]
    public string? MaterialName { get; set; }

    [Required(ErrorMessage = "采购数量不能为空")]
    [Range(1, 100000, ErrorMessage = "采购数量必须在1-100000之间")]
    public int PurchaseQuantity { get; set; }

    [StringLength(100, ErrorMessage = "批次号长度不能超过100个字符")]
    public string? BatchNo { get; set; }

    [StringLength(100, ErrorMessage = "批次1长度不能超过100个字符")]
    public string? Batch1 { get; set; }

    [StringLength(100, ErrorMessage = "批次2长度不能超过100个字符")]
    public string? Batch2 { get; set; }

    [StringLength(100, ErrorMessage = "批次3长度不能超过100个字符")]
    public string? Batch3 { get; set; }

    [StringLength(100, ErrorMessage = "批次4长度不能超过100个字符")]
    public string? Batch4 { get; set; }

    [StringLength(100, ErrorMessage = "批次5长度不能超过100个字符")]
    public string? Batch5 { get; set; }

    [StringLength(100, ErrorMessage = "批次6长度不能超过100个字符")]
    public string? Batch6 { get; set; }

    [StringLength(100, ErrorMessage = "批次7长度不能超过100个字符")]
    public string? Batch7 { get; set; }

    [StringLength(100, ErrorMessage = "批次8长度不能超过100个字符")]
    public string? Batch8 { get; set; }

    [StringLength(100, ErrorMessage = "批次9长度不能超过100个字符")]
    public string? Batch9 { get; set; }

    [StringLength(100, ErrorMessage = "批次10长度不能超过100个字符")]
    public string? Batch10 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注1长度不能超过200个字符")]
    public string? ReserveRemark1 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注2长度不能超过200个字符")]
    public string? ReserveRemark2 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注3长度不能超过200个字符")]
    public string? ReserveRemark3 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注4长度不能超过200个字符")]
    public string? ReserveRemark4 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注5长度不能超过200个字符")]
    public string? ReserveRemark5 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注6长度不能超过200个字符")]
    public string? ReserveRemark6 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注7长度不能超过200个字符")]
    public string? ReserveRemark7 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注8长度不能超过200个字符")]
    public string? ReserveRemark8 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注9长度不能超过200个字符")]
    public string? ReserveRemark9 { get; set; }

    [StringLength(200, ErrorMessage = "预留备注10长度不能超过200个字符")]
    public string? ReserveRemark10 { get; set; }
}

/// <summary>
/// 更新入库单请求
/// </summary>
public class UpdateInboundOrderInput
{
    [Required(ErrorMessage = "入库单ID不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "入库单ID无效")]
    public int Id { get; set; }

    [StringLength(200, ErrorMessage = "供应商名称长度不能超过200个字符")]
    public string? SupplierName { get; set; }

    [StringLength(200, ErrorMessage = "来源长度不能超过200个字符")]
    public string? Source { get; set; }

    [StringLength(50, ErrorMessage = "入库单类型长度不能超过50个字符")]
    public string? OrderType { get; set; }

    [StringLength(50, ErrorMessage = "创建人长度不能超过50个字符")]
    public string? Creator { get; set; }

    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }

    [Required(ErrorMessage = "入库明细不能为空")]
    [MinLength(1, ErrorMessage = "至少需要一行入库明细")]
    public List<CreateInboundOrderItemInput> Items { get; set; } = new();
}

/// <summary>
/// 入库单查询请求
/// </summary>
public class InboundOrderQueryInput
{
    public string? Keyword { get; set; }

    public string? Status { get; set; }

    public string? AuditStatus { get; set; }

    public string? OrderType { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 入库单审核请求
/// </summary>
public class AuditInboundOrderInput
{
    [Required(ErrorMessage = "审核结果不能为空")]
    public bool IsApproved { get; set; }

    [StringLength(50, ErrorMessage = "审核人长度不能超过50个字符")]
    public string? Auditor { get; set; }

    [StringLength(500, ErrorMessage = "审核备注长度不能超过500个字符")]
    public string? AuditRemark { get; set; }
}
