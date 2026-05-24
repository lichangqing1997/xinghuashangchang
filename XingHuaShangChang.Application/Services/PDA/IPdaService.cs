using XingHuaShangChang.Application.Dto;
using XingHuaShangChang.Application.Entity;

namespace XingHuaShangChang.Application.Services.PDA;

/// <summary>
/// PDA服务接口
/// </summary>
public interface IPdaService
{
    Task<string> ScanIn(PdaScanInInput input);
    Task<string> ScanOut(PdaScanOutInput input);
    Task<string> ShelfConfirm(PdaShelfInput input);
    Task<string> MobileReceive(MobileReceiveInput input);
    Task<(List<ScanRecord> Items, int Total)> GetScanRecords(
        string? barcode = null,
        string? operationType = null,
        int pageIndex = 1,
        int pageSize = 20);
}
