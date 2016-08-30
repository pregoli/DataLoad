using System.Web.Mvc;
using PaoloTestMS.Entities.ViewModel;
using PaoloTestMS.Services.Interfaces;

namespace PaoloTestMS.Controllers
{
    [RoutePrefix("")]
    public class DataLoadController : Controller
    {
        private readonly IFilesService filesService;
        private readonly IDownloadService downloadService;

        public DataLoadController(IFilesService filesService, IDownloadService downloadService)
        {
            this.filesService = filesService;
            this.downloadService = downloadService;
        }
        
        public ActionResult Index()
        {
            var model = filesService.GetDirectoryFiles(Configuration.Configuration.DirectorySettings.DirectoryFilesPath);

            return View(model);
        }

        // GET: DataLoad/Details/5
        [HttpGet]
        public ActionResult Details(int auditID, double timeElapsed)
        {
            var model = downloadService.GetAudit(auditID, Configuration.Configuration.DirectorySettings.ConnectionString);
            model.TimeElapsed = timeElapsed;


            return View(model);
        }

        // GET: DataLoad/Create
        public ActionResult Create()
        {
            return View();
        }
    }
}
