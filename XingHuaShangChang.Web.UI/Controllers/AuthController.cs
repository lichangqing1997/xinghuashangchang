using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    // 模拟用户数据（与数据库 User 表对齐）
    private static readonly Dictionary<string, (int Id, string Password, string Name, string Role)> Users = new()
    {
        { "admin", (1, "admin123", "管理员", "admin") },
        { "warehouse", (2, "warehouse123", "仓库管理员", "warehouse") },
        { "operator", (3, "operator123", "操作员", "operator") }
    };

    // token -> 用户名 映射
    private static readonly Dictionary<string, string> TokenStore = new();

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return Ok(Result<object>.Fail("用户名和密码不能为空", "VALIDATION_ERROR"));
        }

        if (!Users.TryGetValue(request.Username, out var user))
        {
            _logger.LogWarning("登录失败：用户不存在 {Username}", request.Username);
            return Ok(Result<object>.Fail("用户名或密码错误", "AUTH_ERROR"));
        }

        if (user.Password != request.Password)
        {
            _logger.LogWarning("登录失败：密码错误 {Username}", request.Username);
            return Ok(Result<object>.Fail("用户名或密码错误", "AUTH_ERROR"));
        }

        // 生成简单的 token（实际项目应使用 JWT）
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        TokenStore[token] = request.Username;

        _logger.LogInformation("用户 {Username} 登录成功", request.Username);

        return Ok(Result<object>.Ok(new
        {
            id = user.Id,
            username = request.Username,
            name = user.Name,
            role = user.Role,
            token = token
        }, "登录成功"));
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        // 从请求头获取 token
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token) || !TokenStore.TryGetValue(token, out var username))
        {
            return Ok(Result<object>.Fail("未登录", "AUTH_ERROR"));
        }

        if (!Users.TryGetValue(username, out var user))
        {
            return Ok(Result<object>.Fail("用户不存在", "AUTH_ERROR"));
        }

        return Ok(Result<object>.Ok(new
        {
            id = user.Id,
            username = username,
            name = user.Name,
            role = user.Role
        }));
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            TokenStore.Remove(token);
        }
        return Ok(Result<object>.Ok(null, "退出成功"));
    }
}

/// <summary>
/// 登录请求
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
