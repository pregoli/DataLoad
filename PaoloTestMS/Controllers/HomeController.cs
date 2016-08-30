using System.Web.Mvc;
using PaoloTestMS.Services.Interfaces;

namespace PaoloTestMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilesService filesService;

        public HomeController(IFilesService filesService)
        {
            this.filesService = filesService;
        }

        public ActionResult Index()
        {
            var files = filesService.GetDirectoryFiles(Configuration.Configuration.DirectorySettings.DirectoryFilesPath);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}