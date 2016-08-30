using System.Web.Http;
using PaoloTestMS.Entities.ViewModel;
using PaoloTestMS.Services.Interfaces;

namespace PaoloTestMS.Controllers.Api
{
    [RoutePrefix("api/download")]
    public class DownloadController : ApiController
    {
        private readonly IDownloadService downloadService;

        public DownloadController(IDownloadService downloadService)
        {
            this.downloadService = downloadService;
        }

        // GET: api/Download
        [AcceptVerbs("GET", "POST")]
        public IHttpActionResult GetAudit(string filename)
        {
            var fileFullPath = $@"{PaoloTestMS.Configuration.Configuration.DirectorySettings.DirectoryFilesPath}\{filename}";
            var result = this.downloadService.Download(fileFullPath, filename, PaoloTestMS.Configuration.Configuration.DirectorySettings.ConnectionString);
            
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
