using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/error")]
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;

        if (exception != null)
        {
            _logger.LogError(exception, "未处理的异常: {Message}", exception.Message);
        }

        return StatusCode(500, Result<object>.Fail(
            exception?.Message ?? "服务器内部错误",
            "INTERNAL_ERROR"
        ));
    }
}
