using PaoloTestMS.Entities.ViewModel;

namespace PaoloTestMS.Services.Interfaces
{
    public interface IDownloadService
    {
        Result Download(string fileFullPath, string filename, string connectionString);

        LoadAudit GetAudit(int auditID, string connectionString);
    }
}
