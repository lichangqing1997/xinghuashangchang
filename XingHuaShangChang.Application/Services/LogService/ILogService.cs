using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.LogService;

public interface ILogService
{
    // 调试日志
    Task<(List<调试日志> Items, int Total)> GetDebugLogs(string? level = null, string? keyword = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 1, int pageSize = 20);
    Task<bool> ClearDebugLogs(DateTime? before = null);

    // 接口日志
    Task<(List<接口日志> Items, int Total)> GetApiLogs(string? method = null, string? keyword = null, int? statusCode = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 1, int pageSize = 20);
    Task<bool> ClearApiLogs(DateTime? before = null);

    // 操作日志
    Task<(List<操作日志> Items, int Total)> GetOperationLogs(string? module = null, string? action = null, string? keyword = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 1, int pageSize = 20);
    Task<bool> ClearOperationLogs(DateTime? before = null);

    // 记录日志
    Task AddDebugLog(string level, string message, string? logger = null, string? exception = null, string? stackTrace = null, string? requestPath = null, string? operatorName = null);
    Task AddApiLog(接口日志 log);
    Task AddOperationLog(操作日志 log);
}
