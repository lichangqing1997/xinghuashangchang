using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.InboundOrderService;

public class InboundOrderService : IInboundOrderService
{
    private readonly ISqlSugarClient _db;

    public InboundOrderService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<InboundOrder> Items, int Total)> GetList(InboundOrderQueryInput query)
    {
        var q = _db.Queryable<InboundOrder>();

        if (!string.IsNullOrEmpty(query.Keyword))
            q = q.Where(x => x.OrderNo.Contains(query.Keyword) ||
                             (x.SupplierName != null && x.SupplierName.Contains(query.Keyword)) ||
                             (x.Source != null && x.Source.Contains(query.Keyword)));
        if (!string.IsNullOrEmpty(query.Status))
            q = q.Where(x => x.Status == query.Status);
        if (!string.IsNullOrEmpty(query.AuditStatus))
            q = q.Where(x => x.AuditStatus == query.AuditStatus);
        if (!string.IsNullOrEmpty(query.OrderType))
            q = q.Where(x => x.OrderType == query.OrderType);
        if (query.StartDate.HasValue)
            q = q.Where(x => x.CreatedAt >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            q = q.Where(x => x.CreatedAt < query.EndDate.Value.AddDays(1));

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<InboundOrder?> GetById(int id)
    {
        return await _db.Queryable<InboundOrder>().InSingleAsync(id);
    }

    public async Task<List<InboundOrderItem>> GetItemsByOrderId(int orderId)
    {
        return await _db.Queryable<InboundOrderItem>()
            .Where(x => x.InboundOrderId == orderId)
            .OrderBy(x => x.SeqNo)
            .ToListAsync();
    }

    public async Task<int> Create(CreateInboundOrderInput input)
    {
        if (!input.Items.Any())
            throw new InvalidOperationException("至少需要一行入库明细");

        try
        {
            _db.Ado.BeginTran();

            // 生成入库单号
            var orderNo = await GenerateOrderNo();

            // 插入入库单
            var order = new InboundOrder
            {
                OrderNo = orderNo,
                SupplierName = input.SupplierName,
                Source = input.Source,
                OrderType = input.OrderType,
                Status = "未处理",
                Creator = input.Creator,
                AuditStatus = "待审核",
                Remark = input.Remark
            };
            var orderId = await _db.Insertable(order).ExecuteReturnIdentityAsync();

            // 插入明细行
            var items = input.Items.Select(itemInput => new InboundOrderItem
            {
                InboundOrderId = orderId,
                SeqNo = itemInput.SeqNo,
                MaterialCode = itemInput.MaterialCode,
                MaterialName = itemInput.MaterialName,
                PurchaseQuantity = itemInput.PurchaseQuantity,
                BatchNo = itemInput.BatchNo,
                Batch1 = itemInput.Batch1,
                Batch2 = itemInput.Batch2,
                Batch3 = itemInput.Batch3,
                Batch4 = itemInput.Batch4,
                Batch5 = itemInput.Batch5,
                Batch6 = itemInput.Batch6,
                Batch7 = itemInput.Batch7,
                Batch8 = itemInput.Batch8,
                Batch9 = itemInput.Batch9,
                Batch10 = itemInput.Batch10,
                ReserveRemark1 = itemInput.ReserveRemark1,
                ReserveRemark2 = itemInput.ReserveRemark2,
                ReserveRemark3 = itemInput.ReserveRemark3,
                ReserveRemark4 = itemInput.ReserveRemark4,
                ReserveRemark5 = itemInput.ReserveRemark5,
                ReserveRemark6 = itemInput.ReserveRemark6,
                ReserveRemark7 = itemInput.ReserveRemark7,
                ReserveRemark8 = itemInput.ReserveRemark8,
                ReserveRemark9 = itemInput.ReserveRemark9,
                ReserveRemark10 = itemInput.ReserveRemark10
            }).ToList();

            await _db.Insertable(items).ExecuteCommandAsync();

            _db.Ado.CommitTran();
            return orderId;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<bool> Update(UpdateInboundOrderInput input)
    {
        var order = await _db.Queryable<InboundOrder>().InSingleAsync(input.Id);
        if (order == null)
            throw new InvalidOperationException("入库单不存在");
        if (order.Status != "未处理")
            throw new InvalidOperationException("只能编辑未处理状态的入库单");

        if (!input.Items.Any())
            throw new InvalidOperationException("至少需要一行入库明细");

        try
        {
            _db.Ado.BeginTran();

            // 更新入库单头
            order.SupplierName = input.SupplierName;
            order.Source = input.Source;
            order.OrderType = input.OrderType;
            order.Creator = input.Creator;
            order.Remark = input.Remark;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.Updateable(order).ExecuteCommandHasChangeAsync();

            // 删除旧明细，插入新明细
            await _db.Deleteable<InboundOrderItem>().Where(x => x.InboundOrderId == input.Id).ExecuteCommandAsync();

            var items = input.Items.Select(itemInput => new InboundOrderItem
            {
                InboundOrderId = input.Id,
                SeqNo = itemInput.SeqNo,
                MaterialCode = itemInput.MaterialCode,
                MaterialName = itemInput.MaterialName,
                PurchaseQuantity = itemInput.PurchaseQuantity,
                BatchNo = itemInput.BatchNo,
                Batch1 = itemInput.Batch1,
                Batch2 = itemInput.Batch2,
                Batch3 = itemInput.Batch3,
                Batch4 = itemInput.Batch4,
                Batch5 = itemInput.Batch5,
                Batch6 = itemInput.Batch6,
                Batch7 = itemInput.Batch7,
                Batch8 = itemInput.Batch8,
                Batch9 = itemInput.Batch9,
                Batch10 = itemInput.Batch10,
                ReserveRemark1 = itemInput.ReserveRemark1,
                ReserveRemark2 = itemInput.ReserveRemark2,
                ReserveRemark3 = itemInput.ReserveRemark3,
                ReserveRemark4 = itemInput.ReserveRemark4,
                ReserveRemark5 = itemInput.ReserveRemark5,
                ReserveRemark6 = itemInput.ReserveRemark6,
                ReserveRemark7 = itemInput.ReserveRemark7,
                ReserveRemark8 = itemInput.ReserveRemark8,
                ReserveRemark9 = itemInput.ReserveRemark9,
                ReserveRemark10 = itemInput.ReserveRemark10
            }).ToList();

            await _db.Insertable(items).ExecuteCommandAsync();

            _db.Ado.CommitTran();
            return true;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        var order = await _db.Queryable<InboundOrder>().InSingleAsync(id);
        if (order == null)
            throw new InvalidOperationException("入库单不存在");
        if (order.Status != "未处理")
            throw new InvalidOperationException("只能删除未处理状态的入库单");

        try
        {
            _db.Ado.BeginTran();

            // 删除明细
            await _db.Deleteable<InboundOrderItem>().Where(x => x.InboundOrderId == id).ExecuteCommandAsync();
            // 删除主表
            await _db.Deleteable<InboundOrder>().Where(x => x.Id == id).ExecuteCommandAsync();

            _db.Ado.CommitTran();
            return true;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<bool> Audit(int orderId, AuditInboundOrderInput input)
    {
        var order = await _db.Queryable<InboundOrder>().InSingleAsync(orderId);
        if (order == null)
            throw new InvalidOperationException("入库单不存在");
        if (order.AuditStatus != "待审核")
            throw new InvalidOperationException("该入库单已审核，不能重复审核");

        order.AuditStatus = input.IsApproved ? "已通过" : "已驳回";
        order.Auditor = input.Auditor;
        order.AuditTime = DateTime.UtcNow;
        order.AuditRemark = input.AuditRemark;
        order.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(order).UpdateColumns(x => new
        {
            x.AuditStatus,
            x.Auditor,
            x.AuditTime,
            x.AuditRemark,
            x.UpdatedAt
        }).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> ChangeStatus(int orderId, string newStatus, string? operatorName)
    {
        var order = await _db.Queryable<InboundOrder>().InSingleAsync(orderId);
        if (order == null)
            throw new InvalidOperationException("入库单不存在");

        // 验证状态流转
        var validTransitions = new Dictionary<string, List<string>>
        {
            { "未处理", new List<string> { "正在处理", "手动关闭" } },
            { "正在处理", new List<string> { "已完成", "手动关闭" } },
        };

        if (!validTransitions.ContainsKey(order.Status) || !validTransitions[order.Status].Contains(newStatus))
            throw new InvalidOperationException($"不能从{order.Status}状态变更为{newStatus}状态");

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        if (newStatus == "已完成")
        {
            order.CompletedAt = DateTime.UtcNow;
            return await _db.Updateable(order).UpdateColumns(x => new
            {
                x.Status,
                x.CompletedAt,
                x.UpdatedAt
            }).ExecuteCommandHasChangeAsync();
        }

        return await _db.Updateable(order).UpdateColumns(x => new
        {
            x.Status,
            x.UpdatedAt
        }).ExecuteCommandHasChangeAsync();
    }

    private async Task<string> GenerateOrderNo()
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var prefix = $"RK{today}";

        var lastOrder = await _db.Queryable<InboundOrder>()
            .Where(x => x.OrderNo.StartsWith(prefix))
            .OrderByDescending(x => x.OrderNo)
            .FirstAsync();

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastSeq = lastOrder.OrderNo.Substring(prefix.Length);
            if (int.TryParse(lastSeq, out int s))
                sequence = s + 1;
        }

        return $"{prefix}{sequence:D4}";
    }
}
