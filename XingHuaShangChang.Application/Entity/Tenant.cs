using SqlSugar;

namespace XingHuaShangChang.Application.Entity;

/// <summary>
/// 租户表
/// </summary>
[SugarTable("Tenants")]
[SugarIndex("idx_tenant_code", nameof(TenantCode), OrderByType.Desc)]
public class Tenant
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 租户编码
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = false)]
    public string TenantCode { get; set; } = string.Empty;

    /// <summary>
    /// 租户名称
    /// </summary>
    [SugarColumn(Length = 200, IsNullable = false)]
    public string TenantName { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Address { get; set; }

    /// <summary>
    /// 数据库连接字符串（独立数据库模式）
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? ConnectionString { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? DatabaseName { get; set; }

    /// <summary>
    /// 最大用户数限制
    /// </summary>
    [SugarColumn(IsNullable = false, DefaultValue = "100")]
    public int MaxUsers { get; set; } = 100;

    /// <summary>
    /// 过期时间
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public DateTime? ExpireAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Remark { get; set; }

    /// <summary>
    /// 状态：1=启用，0=禁用
    /// </summary>
    [SugarColumn(IsNullable = false, DefaultValue = "1")]
    public int Status { get; set; } = 1;

    [SugarColumn(IsNullable = false)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [SugarColumn(IsNullable = true)]
    public DateTime? UpdatedAt { get; set; }
}
