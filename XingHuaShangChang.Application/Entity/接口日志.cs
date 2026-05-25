using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 接口日志表
/// </summary>
[SugarTable("接口日志")]
public class 接口日志
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>
    /// 请求方法（GET/POST/PUT/DELETE）
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = false)]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 请求路径
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// 查询参数
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "text")]
    public string? QueryString { get; set; }

    /// <summary>
    /// 请求体
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext")]
    public string? RequestBody { get; set; }

    /// <summary>
    /// 响应体
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDataType = "longtext")]
    public string? ResponseBody { get; set; }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int StatusCode { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public long Elapsed { get; set; }

    /// <summary>
    /// 客户端IP
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Ip { get; set; }

    /// <summary>
    /// User-Agent
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Operator { get; set; }

    /// <summary>
    /// 是否成功（状态码 200-299）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
