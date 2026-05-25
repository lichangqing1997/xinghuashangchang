using SqlSugar;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.LogService;

public class LogService : ILogService
{
    private readonly ISqlSugarClient _db;

    public LogService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<调试日志> Items, int Total)> GetDebugLogs(string? level = null, string? keyword = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<调试日志>();

        if (!string.IsNullOrEmpty(level))
            query = query.Where(x => x.Level == level);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.Message.Contains(keyword) || (x.Logger != null && x.Logger.Contains(keyword)));

        if (startTime.HasValue)
            query = query.Where(x => x.CreatedAt >= startTime.Value);

        if (endTime.HasValue)
            query = query.Where(x => x.CreatedAt <= endTime.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> ClearDebugLogs(DateTime? before = null)
    {
        if (before.HasValue)
            return await _db.Deleteable<调试日志>().Where(x => x.CreatedAt < before.Value).ExecuteCommandHasChangeAsync();
        return await _db.Deleteable<调试日志>().ExecuteCommandHasChangeAsync();
    }

    public async Task<(List<接口日志> Items, int Total)> GetApiLogs(string? method = null, string? keyword = null, int? statusCode = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<接口日志>();

        if (!string.IsNullOrEmpty(method))
            query = query.Where(x => x.Method == method.ToUpper());

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.Path.Contains(keyword) || (x.Operator != null && x.Operator.Contains(keyword)));

        if (statusCode.HasValue)
            query = query.Where(x => x.StatusCode == statusCode.Value);

        if (startTime.HasValue)
            query = query.Where(x => x.CreatedAt >= startTime.Value);

        if (endTime.HasValue)
            query = query.Where(x => x.CreatedAt <= endTime.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> ClearApiLogs(DateTime? before = null)
    {
        if (before.HasValue)
            return await _db.Deleteable<接口日志>().Where(x => x.CreatedAt < before.Value).ExecuteCommandHasChangeAsync();
        return await _db.Deleteable<接口日志>().ExecuteCommandHasChangeAsync();
    }

    public async Task<(List<操作日志> Items, int Total)> GetOperationLogs(string? module = null, string? action = null, string? keyword = null, DateTime? startTime = null, DateTime? endTime = null, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<操作日志>();

        if (!string.IsNullOrEmpty(module))
            query = query.Where(x => x.Module == module);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(x => x.Action == action);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => (x.Description != null && x.Description.Contains(keyword)) || (x.Operator != null && x.Operator.Contains(keyword)));

        if (startTime.HasValue)
            query = query.Where(x => x.CreatedAt >= startTime.Value);

        if (endTime.HasValue)
            query = query.Where(x => x.CreatedAt <= endTime.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<bool> ClearOperationLogs(DateTime? before = null)
    {
        if (before.HasValue)
            return await _db.Deleteable<操作日志>().Where(x => x.CreatedAt < before.Value).ExecuteCommandHasChangeAsync();
        return await _db.Deleteable<操作日志>().ExecuteCommandHasChangeAsync();
    }

    public async Task AddDebugLog(string level, string message, string? logger = null, string? exception = null, string? stackTrace = null, string? requestPath = null, string? operatorName = null)
    {
        var log = new 调试日志
        {
            Level = level,
            Logger = logger,
            Message = message,
            Exception = exception,
            StackTrace = stackTrace,
            RequestPath = requestPath,
            Operator = operatorName,
            CreatedAt = DateTime.UtcNow
        };
        await _db.Insertable(log).ExecuteCommandAsync();
    }

    public async Task AddApiLog(接口日志 log)
    {
        log.CreatedAt = DateTime.UtcNow;
        await _db.Insertable(log).ExecuteCommandAsync();
    }

    public async Task AddOperationLog(操作日志 log)
    {
        log.CreatedAt = DateTime.UtcNow;
        await _db.Insertable(log).ExecuteCommandAsync();
    }
}
