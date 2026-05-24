using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.CompanyService;

public interface ICompanyService
{
    Task<(List<Company> Items, int Total)> GetList(string? keyword, int pageIndex = 1, int pageSize = 20);
    Task<Company?> GetById(int id);
    Task<List<CompanyDetail>> GetDetailsByCompanyId(int companyId);
    Task<int> Create(CreateCompanyInput input);
    Task<bool> Update(UpdateCompanyInput input);
    Task<bool> Delete(int id);
}
