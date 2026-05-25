using System.Diagnostics;
using System.Text;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.LogService;

namespace XingHuaShangChang.Web.UI.Middleware;

/// <summary>
/// API接口日志中间件
/// </summary>
public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiLoggingMiddleware> _logger;

    // 不记录日志的路径前缀
    private static readonly string[] _excludePaths = { "/swagger", "/health", "/api/Log/" };

    public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 排除不需要记录的路径
        var path = context.Request.Path.Value ?? "";
        if (_excludePaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        // 只记录API请求
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var log = new 接口日志
        {
            Method = context.Request.Method,
            Path = path,
            QueryString = context.Request.QueryString.ToString(),
            Ip = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent.ToString()
        };

        // 获取操作人
        var username = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(username))
            log.Operator = username;

        // 读取请求体
        try
        {
            if (context.Request.ContentLength > 0 && context.Request.ContentLength < 1024 * 100) // 限制100KB
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                log.RequestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "读取请求体失败");
        }

        // 替换响应流以捕获响应体
        var originalResponseBody = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            log.StatusCode = context.Response.StatusCode;
            log.Elapsed = stopwatch.ElapsedMilliseconds;
            log.IsSuccess = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;

            // 读取响应体
            try
            {
                responseBodyStream.Position = 0;
                var responseText = await new StreamReader(responseBodyStream, Encoding.UTF8).ReadToEndAsync();
                log.ResponseBody = responseText.Length > 2000 ? responseText[..2000] + "..." : responseText;
            }
            catch
            {
                // 忽略响应体读取失败
            }

            // 将响应内容复制回原始流
            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            // 异步保存日志
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = context.RequestServices.CreateScope();
                    var logService = scope.ServiceProvider.GetRequiredService<ILogService>();
                    await logService.AddApiLog(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "保存接口日志失败");
                }
            });
        }
    }
}
