namespace XingHuaShangChang.Common.Util;

/// <summary>
/// 统一返回结果
/// </summary>
public class Result<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误码（可选）
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static Result<T> Ok(T data, string message = "操作成功")
    {
        return new Result<T> { Success = true, Message = message, Data = data };
    }

    public static Result<T> Fail(string message = "操作失败", string? errorCode = null)
    {
        return new Result<T> { Success = false, Message = message, ErrorCode = errorCode };
    }
}

/// <summary>
/// 分页结果
/// </summary>
public class PageResult<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;
}
