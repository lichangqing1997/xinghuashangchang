using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Services.UserService;
using XingHuaShangChang.Common.Util;

namespace XingHuaShangChang.Web.UI.Controllers.User;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    [HttpGet("list")]
    public async Task<Result<PageResult<UserDetailOutput>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] int status = -1,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var (items, total) = await _userService.GetList(keyword, status, pageIndex, pageSize);
            var pageResult = new PageResult<UserDetailOutput>
            {
                Items = items,
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            return Result<PageResult<UserDetailOutput>>.Ok(pageResult);
        }
        catch (Exception ex)
        {
            return Result<PageResult<UserDetailOutput>>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Result<UserDetailOutput>> GetById(int id)
    {
        try
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return Result<UserDetailOutput>.Fail("用户不存在", "NOT_FOUND");

            return Result<UserDetailOutput>.Ok(user);
        }
        catch (Exception ex)
        {
            return Result<UserDetailOutput>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateUserInput input)
    {
        try
        {
            var userId = await _userService.Create(input);
            return Result<int>.Ok(userId, "用户创建成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<int>.Fail(ex.Message, "VALIDATION_ERROR");
        }
        catch (Exception ex)
        {
            return Result<int>.Fail(ex.Message, "CREATE_ERROR");
        }
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut]
    public async Task<Result<bool>> Update([FromBody] UpdateUserInput input)
    {
        try
        {
            var result = await _userService.Update(input);
            return Result<bool>.Ok(result, "用户更新成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(ex.Message, "UPDATE_ERROR");
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<Result<bool>> Delete(int id)
    {
        try
        {
            var result = await _userService.Delete(id);
            return Result<bool>.Ok(result, "用户删除成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(ex.Message, "DELETE_ERROR");
        }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    [HttpPost("change-password")]
    public async Task<Result<bool>> ChangePassword([FromBody] ChangePasswordInput input)
    {
        try
        {
            var result = await _userService.ChangePassword(input);
            return Result<bool>.Ok(result, "密码修改成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Fail(ex.Message, "VALIDATION_ERROR");
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(ex.Message, "UPDATE_ERROR");
        }
    }

    /// <summary>
    /// 上传头像
    /// </summary>
    [HttpPost("avatar")]
    public async Task<Result<string>> UploadAvatar(IFormFile file, [FromQuery] int userId)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Result<string>.Fail("请选择文件", "VALIDATION_ERROR");

            // 验证文件类型
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return Result<string>.Fail("只支持 JPG、PNG、GIF、WEBP 格式", "VALIDATION_ERROR");

            // 限制文件大小 5MB
            if (file.Length > 5 * 1024 * 1024)
                return Result<string>.Fail("文件大小不能超过 5MB", "VALIDATION_ERROR");

            // 保存文件
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"avatar_{userId}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
            var relativePath = Path.Combine("uploads", "avatars", fileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            var dir = Path.GetDirectoryName(fullPath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 更新用户头像路径
            var avatarUrl = $"/{relativePath.Replace("\\", "/")}";
            await _userService.UpdateAvatar(userId, avatarUrl);

            return Result<string>.Ok(avatarUrl, "头像上传成功");
        }
        catch (InvalidOperationException ex)
        {
            return Result<string>.Fail(ex.Message, "VALIDATION_ERROR");
        }
        catch (Exception ex)
        {
            return Result<string>.Fail($"上传失败：{ex.Message}", "UPLOAD_ERROR");
        }
    }
}
