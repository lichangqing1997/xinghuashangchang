using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Application.Services.FileService;

/// <summary>
/// 文件服务实现
/// </summary>
public class FileService : IFileService
{
    private readonly ISqlSugarClient _db;
    private readonly IWebHostEnvironment _env;

    // 允许的文件类型
    private static readonly Dictionary<string, string[]> AllowedFileTypes = new()
    {
        { "image", new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" } },
        { "document", new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" } },
        { "apk", new[] { ".apk" } }
    };

    // 最大文件大小：50MB
    private const long MaxFileSize = 50 * 1024 * 1024;

    public FileService(ISqlSugarClient db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<FileAttachment> UploadAsync(IFormFile file, string? businessType, int? businessId, string? operatorName)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("请选择要上传的文件");

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException("文件大小不能超过50MB");

        // 确定文件类型
        var fileType = GetFileType(file.FileName);
        if (fileType == null)
            throw new InvalidOperationException("不支持的文件类型");

        // 生成存储路径
        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var datePath = DateTime.UtcNow.ToString("yyyy/MM");
        var relativePath = Path.Combine("uploads", datePath, storedFileName);
        var fullPath = Path.Combine(_env.WebRootPath, relativePath);

        // 确保目录存在
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // 保存文件
        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 创建文件记录
        var attachment = new FileAttachment
        {
            FileName = file.FileName,
            StoredFileName = storedFileName,
            FilePath = relativePath.Replace("\\", "/"),
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileType = fileType,
            BusinessType = businessType,
            BusinessId = businessId,
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = operatorName
        };

        // 如果是APK文件，解析版本信息
        if (fileType == "apk")
        {
            var (version, versionCode) = await ParseApkVersionAsync(fullPath);
            attachment.Version = version;
            attachment.VersionCode = versionCode;
        }

        // 保存到数据库
        var id = await _db.Insertable(attachment).ExecuteReturnIdentityAsync();
        attachment.Id = id;

        return attachment;
    }

    public async Task<FileAttachment?> GetByIdAsync(int id)
    {
        return await _db.Queryable<FileAttachment>()
            .Where(f => f.Id == id && f.Status == 1)
            .FirstAsync();
    }

    public async Task<PageResult<FileAttachment>> GetListAsync(string? fileType, string? businessType, int pageIndex, int pageSize)
    {
        var total = new RefAsync<int>();

        var query = _db.Queryable<FileAttachment>()
            .Where(f => f.Status == 1);

        if (!string.IsNullOrEmpty(fileType))
            query = query.Where(f => f.FileType == fileType);

        if (!string.IsNullOrEmpty(businessType))
            query = query.Where(f => f.BusinessType == businessType);

        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .ToPageListAsync(pageIndex, pageSize, total);

        return new PageResult<FileAttachment>
        {
            Items = items,
            Total = total.Value,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var file = await _db.Queryable<FileAttachment>()
            .Where(f => f.Id == id && f.Status == 1)
            .FirstAsync();

        if (file == null)
            return false;

        // 软删除
        file.Status = 0;
        await _db.Updateable(file).ExecuteCommandAsync();

        // 可选：物理删除文件
        // var fullPath = Path.Combine(_env.WebRootPath, file.FilePath);
        // if (File.Exists(fullPath))
        //     File.Delete(fullPath);

        return true;
    }

    public async Task<FileAttachment?> GetLatestApkAsync()
    {
        return await _db.Queryable<FileAttachment>()
            .Where(f => f.FileType == "apk" && f.Status == 1)
            .OrderByDescending(f => f.VersionCode)
            .FirstAsync();
    }

    /// <summary>
    /// 根据文件扩展名判断文件类型
    /// </summary>
    private static string? GetFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        if (string.IsNullOrEmpty(extension))
            return null;

        foreach (var (type, extensions) in AllowedFileTypes)
        {
            if (extensions.Contains(extension))
                return type;
        }

        return "other";
    }

    /// <summary>
    /// 解析APK文件的版本信息
    /// </summary>
    private static async Task<(string? version, int? versionCode)> ParseApkVersionAsync(string apkPath)
    {
        try
        {
            await using var stream = new FileStream(apkPath, FileMode.Open, FileAccess.Read);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            // 查找 AndroidManifest.xml
            var manifestEntry = archive.GetEntry("AndroidManifest.xml");
            if (manifestEntry == null)
                return (null, null);

            await using var manifestStream = manifestEntry.Open();
            using var memoryStream = new MemoryStream();
            await manifestStream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            // AndroidManifest.xml 在 APK 中是二进制 XML 格式
            // 尝试从二进制数据中提取版本信息
            var content = System.Text.Encoding.UTF8.GetString(bytes);

            // 尝试匹配 versionName 和 versionCode
            var versionName = ExtractXmlAttribute(content, "versionName");
            var versionCodeStr = ExtractXmlAttribute(content, "versionCode");

            int? versionCode = null;
            if (int.TryParse(versionCodeStr, out var vc))
                versionCode = vc;

            return (versionName, versionCode);
        }
        catch
        {
            // 解析失败时返回空
            return (null, null);
        }
    }

    /// <summary>
    /// 从二进制XML中提取属性值（简化实现）
    /// </summary>
    private static string? ExtractXmlAttribute(string content, string attributeName)
    {
        // 尝试多种模式匹配
        var patterns = new[]
        {
            $@"{attributeName}=""([^""]+)""",
            $@"{attributeName}=(\d+)",
            $@"android:{attributeName}=""([^""]+)"""
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(content, pattern);
            if (match.Success)
                return match.Groups[1].Value;
        }

        return null;
    }
}
