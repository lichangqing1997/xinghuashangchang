using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 调试日志表
/// </summary>
[SugarTable("调试日志")]
public class 调试日志
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>
    /// 日志级别（Debug/Info/Warn/Error/Fatal）
    /// </summary>
    [SugarColumn(Length = 20, IsNullable = false)]
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// 日志来源
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Logger { get; set; }

    /// <summary>
    /// 日志消息
    /// </summary>
    [SugarColumn(IsNullable = false, ColumnDataType = "text")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext")]
    public string? Exception { get; set; }

    /// <summary>
    /// 堆栈跟踪
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? RequestPath { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Operator { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
