using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 入库单
/// </summary>
[SugarTable("InboundOrders")]
[SugarIndex("idx_inbound_order_no", nameof(OrderNo), OrderByType.Desc)]
public class InboundOrder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 入库单编号
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 供应商名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? SupplierName { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Source { get; set; }

    /// <summary>
    /// 入库单类型
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? OrderType { get; set; }

    /// <summary>
    /// 状态（未处理/正在处理/已完成/手动关闭）
    /// </summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "未处理";

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Creator { get; set; }

    /// <summary>
    /// 审核状态
    /// </summary>
    [SugarColumn(Length = 20)]
    public string AuditStatus { get; set; } = "待审核";

    /// <summary>
    /// 审核人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Auditor { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? AuditRemark { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 入库完成时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? CompletedAt { get; set; }
}
