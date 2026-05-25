using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 操作日志表
/// </summary>
[SugarTable("操作日志")]
public class 操作日志
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// 操作类型（新增/编辑/删除/查询/导出等）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 操作描述
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "text")]
    public string? Description { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = true)]
    public string? Method { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? RequestPath { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext")]
    public string? RequestParams { get; set; }

    /// <summary>
    /// 响应结果
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext")]
    public string? ResponseResult { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Operator { get; set; }

    /// <summary>
    /// 客户端IP
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Ip { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "text")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public long Elapsed { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
