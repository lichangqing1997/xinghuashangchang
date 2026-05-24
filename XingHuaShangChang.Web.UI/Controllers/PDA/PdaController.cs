using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.PDA;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.PDA;

[ApiController]
[Route("api/[controller]")]
public class PdaController : ControllerBase
{
    private readonly IPdaService _service;

    private static readonly string[] SuccessKeywords = { "成功" };

    public PdaController(IPdaService service)
    {
        _service = service;
    }

    [HttpPost("scan-in")]
    public async Task<Result<string>> ScanIn([FromBody] PdaScanInInput input)
    {
        var result = await _service.ScanIn(input);
        return IsSuccess(result)
            ? Result<string>.Ok(result)
            : Result<string>.Fail(result);
    }

    [HttpPost("scan-out")]
    public async Task<Result<string>> ScanOut([FromBody] PdaScanOutInput input)
    {
        var result = await _service.ScanOut(input);
        return IsSuccess(result)
            ? Result<string>.Ok(result)
            : Result<string>.Fail(result);
    }

    [HttpPost("shelf")]
    public async Task<Result<string>> ShelfConfirm([FromBody] PdaShelfInput input)
    {
        var result = await _service.ShelfConfirm(input);
        return IsSuccess(result)
            ? Result<string>.Ok(result)
            : Result<string>.Fail(result);
    }

    [HttpPost("mobile-receive")]
    public async Task<Result<string>> MobileReceive([FromBody] MobileReceiveInput input)
    {
        var result = await _service.MobileReceive(input);
        return IsSuccess(result)
            ? Result<string>.Ok(result)
            : Result<string>.Fail(result);
    }

    [HttpGet("records")]
    public async Task<Result<PageResult<ScanRecord>>> GetRecords(
        [FromQuery] string? barcode,
        [FromQuery] string? operationType,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, 100);
        var (items, total) = await _service.GetScanRecords(barcode, operationType, pageIndex, pageSize);
        var pageResult = new PageResult<ScanRecord>
        {
            Items = items,
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Result<PageResult<ScanRecord>>.Ok(pageResult);
    }

    private static bool IsSuccess(string message)
    {
        return SuccessKeywords.Any(k => message.EndsWith(k) || message == k);
    }
}
