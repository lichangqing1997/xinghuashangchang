using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 文件附件表
/// </summary>
[SugarTable("FileAttachments")]
[SugarIndex("idx_file_type", nameof(FileType), OrderByType.Desc)]
[SugarIndex("idx_business", nameof(BusinessType), OrderByType.Desc)]
public class FileAttachment
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 原始文件名
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 存储文件名（GUID）
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// 存储路径（相对路径）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = false)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// MIME类型
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = false)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public long FileSize { get; set; }

    /// <summary>
    /// 文件分类：image/document/apk/other
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string FileType { get; set; } = "other";

    /// <summary>
    /// 业务类型：product/supplier/avatar等
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? BusinessType { get; set; }

    /// <summary>
    /// 关联业务ID
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? BusinessId { get; set; }

    /// <summary>
    /// APK版本号（仅APK文件）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Version { get; set; }

    /// <summary>
    /// APK版本代码（仅APK文件）
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public int? VersionCode { get; set; }

    /// <summary>
    /// 状态：0-禁用 1-启用
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 创建人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? CreatedBy { get; set; }
}
