using Microsoft.AspNetCore.Mvc;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Services.MenuService;
using XingHuaShangChang.Common.Util;
using MenuEntity = XingHuaShangChang.Application.Entity.Menu;

namespace XingHuaShangChang.Web.UI.Controllers.Menu;

/// <summary>
/// 菜单管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取菜单列表（平铺）
    /// </summary>
    [HttpGet("list")]
    public async Task<Result<List<MenuEntity>>> GetList(
        [FromQuery] string? keyword,
        [FromQuery] int status = -1)
    {
        try
        {
            var menus = await _menuService.GetList(keyword, status);
            return Result<List<MenuEntity>>.Ok(menus);
        }
        catch (Exception ex)
        {
            return Result<List<MenuEntity>>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    [HttpGet("tree")]
    public async Task<Result<List<MenuTreeOutput>>> GetTree(
        [FromQuery] string? keyword,
        [FromQuery] int status = -1)
    {
        try
        {
            var tree = await _menuService.GetTree(keyword, status);
            return Result<List<MenuTreeOutput>>.Ok(tree);
        }
        catch (Exception ex)
        {
            return Result<List<MenuTreeOutput>>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取当前用户的菜单树
    /// </summary>
    [HttpGet("user-tree/{userId}")]
    public async Task<Result<List<MenuTreeOutput>>> GetUserTree(int userId)
    {
        try
        {
            var tree = await _menuService.GetTreeByUserId(userId);
            return Result<List<MenuTreeOutput>>.Ok(tree);
        }
        catch (Exception ex)
        {
            return Result<List<MenuTreeOutput>>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<Result<MenuEntity>> GetById(int id)
    {
        try
        {
            var menu = await _menuService.GetById(id);
            if (menu == null)
                return Result<MenuEntity>.Fail("菜单不存在", "NOT_FOUND");

            return Result<MenuEntity>.Ok(menu);
        }
        catch (Exception ex)
        {
            return Result<MenuEntity>.Fail(ex.Message, "QUERY_ERROR");
        }
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    public async Task<Result<int>> Create([FromBody] CreateMenuInput input)
    {
        try
        {
            var menuId = await _menuService.Create(input);
            return Result<int>.Ok(menuId, "菜单创建成功");
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
    /// 更新菜单
    /// </summary>
    [HttpPut]
    public async Task<Result<bool>> Update([FromBody] UpdateMenuInput input)
    {
        try
        {
            var result = await _menuService.Update(input);
            return Result<bool>.Ok(result, "菜单更新成功");
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
    /// 删除菜单
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<Result<bool>> Delete(int id)
    {
        try
        {
            var result = await _menuService.Delete(id);
            return Result<bool>.Ok(result, "菜单删除成功");
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
