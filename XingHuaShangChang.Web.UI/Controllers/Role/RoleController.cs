using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Services.RoleService;
using XingHuaShangChang.Common.Util;
using RoleEntity = XingHuaShangChang.Application.Entity.Role;

namespace XingHuaShangChang.Web.UI.Controllers.Role;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpGet("list")]
    public async Task<Result<PageResult<RoleDetailOutput>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] int status = -1,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            pageSize = Math.Min(pageSize, 100);
            var (items, total) = await _roleService.GetList(keyword, status, pageIndex, pageSize);
            var pageResult = new PageResult<RoleDetailOutput>
            {
                Items = items,
                Total = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            return Result<PageResult<RoleDetailOutput>>.Ok(pageResult);
        }
        catch (Exception ex)
        {
            return Result<PageResult<RoleDetailOutput>>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取所有启用的角色
    /// </summary>
    [HttpGet("all")]
    public async Task<Result<List<RoleEntity>>> GetAll()
    {
        try
        {
            var roles = await _roleService.GetAllActive();
            return Result<List<RoleEntity>>.Ok(roles);
        }
        catch (Exception ex)
        {
            return Result<List<RoleEntity>>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Result<RoleDetailOutput>> GetById(int id)
    {
        try
        {
            var role = await _roleService.GetById(id);
            if (role == null)
                return Result<RoleDetailOutput>.Fail("角色不存在", "NOT_FOUND");

            return Result<RoleDetailOutput>.Ok(role);
        }
        catch (Exception ex)
        {
            return Result<RoleDetailOutput>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateRoleInput input)
    {
        try
        {
            var roleId = await _roleService.Create(input);
            return Result<int>.Ok(roleId, "角色创建成功");
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
    /// 更新角色
    /// </summary>
    [HttpPut]
    public async Task<Result<bool>> Update([FromBody] UpdateRoleInput input)
    {
        try
        {
            var result = await _roleService.Update(input);
            return Result<bool>.Ok(result, "角色更新成功");
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
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<Result<bool>> Delete(int id)
    {
        try
        {
            var result = await _roleService.Delete(id);
            return Result<bool>.Ok(result, "角色删除成功");
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
}
