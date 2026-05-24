namespace XingHuaShangChang.Application.Const;

/// <summary>
/// 进货单状态
/// </summary>
public static class 进货单状态
{
    public const string 待收货 = "待收货";
    public const string 已收货 = "已收货";
    public const string 已入库 = "已入库";
    public const string 已取消 = "已取消";
}

/// <summary>
/// 销售单状态
/// </summary>
public static class 销售单状态
{
    public const string 待出库 = "待出库";
    public const string 已出库 = "已出库";
    public const string 已完成 = "已完成";
    public const string 已退货 = "已退货";
    public const string 已取消 = "已取消";
}

/// <summary>
/// 出库单状态
/// </summary>
public static class 出库单状态
{
    public const string 待出库 = "待出库";
    public const string 已出库 = "已出库";
    public const string 已取消 = "已取消";
}

/// <summary>
/// 库位状态
/// </summary>
public static class 库位状态
{
    public const string 空闲 = "空闲";
    public const string 占用 = "占用";
    public const string 锁定 = "锁定";
}

/// <summary>
/// PDA操作类型
/// </summary>
public static class PDA操作类型
{
    public const string 入库 = "入库";
    public const string 出库 = "出库";
    public const string 上架 = "上架";
}

/// <summary>
/// 商品类别
/// </summary>
public static class 商品类别
{
    public const string 衣服 = "衣服";
    public const string 鞋子 = "鞋子";
    public const string 裤子 = "裤子";
    public const string 配件 = "配件";
    public const string 其他 = "其他";
}
