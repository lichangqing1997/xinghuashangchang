using Microsoft.AspNetCore.Http;
using XingHuaShangChang.Application.Entity;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Application.Services.FileService;

/// <summary>
/// 文件服务接口
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="businessId">业务ID</param>
    /// <param name="operatorName">操作人</param>
    /// <returns>文件附件信息</returns>
    Task<FileAttachment> UploadAsync(IFormFile file, string? businessType, int? businessId, string? operatorName);

    /// <summary>
    /// 根据ID获取文件信息
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns>文件附件信息</returns>
    Task<FileAttachment?> GetByIdAsync(int id);

    /// <summary>
    /// 获取文件列表
    /// </summary>
    /// <param name="fileType">文件类型</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>分页结果</returns>
    Task<PageResult<FileAttachment>> GetListAsync(string? fileType, string? businessType, int pageIndex, int pageSize);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 获取最新APK文件
    /// </summary>
    /// <returns>最新APK文件信息</returns>
    Task<FileAttachment?> GetLatestApkAsync();
}
