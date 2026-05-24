using SqlSugar;
using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.CompanyService;

public class CompanyService : ICompanyService
{
    private readonly ISqlSugarClient _db;

    public CompanyService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<(List<Company> Items, int Total)> GetList(string? keyword, int pageIndex = 1, int pageSize = 20)
    {
        var query = _db.Queryable<Company>().Where(x => x.Status == 1);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.CompanyName.Contains(keyword) || (x.CompanyCode != null && x.CompanyCode.Contains(keyword)) || (x.BusinessLicenseNo != null && x.BusinessLicenseNo.Contains(keyword)));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Company?> GetById(int id)
    {
        return await _db.Queryable<Company>().Where(x => x.Id == id && x.Status == 1).FirstAsync();
    }

    public async Task<List<CompanyDetail>> GetDetailsByCompanyId(int companyId)
    {
        return await _db.Queryable<CompanyDetail>()
            .Where(x => x.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<int> Create(CreateCompanyInput input)
    {
        try
        {
            _db.Ado.BeginTran();

            var company = new Company
            {
                CompanyName = input.CompanyName,
                LegalPerson = input.LegalPerson,
                BusinessLicenseNo = input.BusinessLicenseNo,
                CompanyCode = input.CompanyCode
            };
            var companyId = await _db.Insertable(company).ExecuteReturnIdentityAsync();

            if (input.Details != null && input.Details.Any())
            {
                var details = input.Details.Select(d => new CompanyDetail
                {
                    CompanyId = companyId,
                    FieldName = d.FieldName,
                    FieldValue = d.FieldValue
                }).ToList();
                await _db.Insertable(details).ExecuteCommandAsync();
            }

            _db.Ado.CommitTran();
            return companyId;
        }
        catch (Exception)
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }

    public async Task<bool> Update(UpdateCompanyInput input)
    {
        var company = await _db.Queryable<Company>().Where(x => x.Id == input.Id && x.Status == 1).FirstAsync();
        if (company == null)
            throw new InvalidOperationException("公司不存在");

        try
        {
            _db.Ado.BeginTran();

            company.CompanyName = input.CompanyName;
            company.LegalPerson = input.LegalPerson;
            company.BusinessLicenseNo = input.BusinessLicenseNo;
            company.CompanyCode = input.CompanyCode;
            company.UpdatedAt = DateTime.UtcNow;
            await _db.Updateable(company).ExecuteCommandHasChangeAsync();

            // 替换扩展信息
            await _db.Deleteable<CompanyDetail>().Where(x => x.CompanyId == input.Id).ExecuteCommandAsync();
            if (input.Details != null && input.Details.Any())
            {
                var details = input.Details.Select(d => new CompanyDetail
                {
                    CompanyId = input.Id,
                    FieldName = d.FieldName,
                    FieldValue = d.FieldValue
                }).ToList();
                await _db.Insertable(details).ExecuteCommandAsync();
            }

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
        var company = await _db.Queryable<Company>().Where(x => x.Id == id && x.Status == 1).FirstAsync();
        if (company == null)
            throw new InvalidOperationException("公司不存在");

        company.Status = 0;
        company.UpdatedAt = DateTime.UtcNow;
        return await _db.Updateable(company).UpdateColumns(x => new { x.Status, x.UpdatedAt }).ExecuteCommandHasChangeAsync();
    }
}
