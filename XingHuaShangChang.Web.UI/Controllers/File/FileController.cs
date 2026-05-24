using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Application.Services.FileService;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.File;

/// <summary>
/// 文件管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="businessId">业务ID</param>
    /// <returns>文件信息</returns>
    [HttpPost("upload")]
    public async Task<Result<FileAttachment>> Upload(
        IFormFile file,
        [FromQuery] string? businessType,
        [FromQuery] int? businessId)
    {
        try
        {
            var operatorName = HttpContext.Request.Headers["X-Operator-Name"].FirstOrDefault() ?? "system";
            var result = await _fileService.UploadAsync(file, businessType, businessId, operatorName);
            return Result<FileAttachment>.Ok(result, "上传成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<FileAttachment>.Fail(ex.Message, "VALIDATION_ERROR");
        }
        catch (Exception ex)
        {
            return Result<FileAttachment>.Fail($"上传失败：{ex.Message}", "UPLOAD_ERROR");
        }
    }

    /// <summary>
    /// 获取文件列表
    /// </summary>
    /// <param name="fileType">文件类型</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>文件列表</returns>
    [HttpGet("list")]
    public async Task<Result<PageResult<FileAttachment>>> GetList(
        [FromQuery] string? fileType,
        [FromQuery] string? businessType,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var result = await _fileService.GetListAsync(fileType, businessType, pageIndex, pageSize);
            return Result<PageResult<FileAttachment>>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<PageResult<FileAttachment>>.Fail($"查询失败：{ex.Message}", "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取文件详情
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns>文件信息</returns>
    [HttpGet("{id}")]
    public async Task<Result<FileAttachment>> GetById(int id)
    {
        try
        {
            var file = await _fileService.GetByIdAsync(id);
            if (file == null)
                return Result<FileAttachment>.Fail("未找到文件", "NOT_FOUND");
            return Result<FileAttachment>.Ok(file);
        }
        catch (Exception ex)
        {
            return Result<FileAttachment>.Fail($"查询失败：{ex.Message}", "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns>文件流</returns>
    [HttpGet("download/{id}")]
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var file = await _fileService.GetByIdAsync(id);
            if (file == null)
                return NotFound("文件不存在");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("文件不存在");

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, file.ContentType, file.FileName);
        }
        catch
        {
            return StatusCode(500, "下载失败");
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns>是否成功</returns>
    [HttpDelete("{id}")]
    public async Task<Result<bool>> Delete(int id)
    {
        try
        {
            var result = await _fileService.DeleteAsync(id);
            return result ? Result<bool>.Ok(true, "删除成功") : Result<bool>.Fail("删除失败", "DELETE_ERROR");
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"删除失败：{ex.Message}", "DELETE_ERROR");
        }
    }

    /// <summary>
    /// 获取最新APK信息
    /// </summary>
    /// <returns>最新APK文件信息</returns>
    [HttpGet("latest-apk")]
    public async Task<Result<FileAttachment>> GetLatestApk()
    {
        try
        {
            var file = await _fileService.GetLatestApkAsync();
            if (file == null)
                return Result<FileAttachment>.Fail("暂无APK文件", "NOT_FOUND");
            return Result<FileAttachment>.Ok(file);
        }
        catch (Exception ex)
        {
            return Result<FileAttachment>.Fail($"查询失败：{ex.Message}", "QUERY_ERROR");
        }
    }
}
