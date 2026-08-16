using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace PortFolioAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult NotFoundPage()
        {
            return View();
        }

        [HttpGet("/project-screenshots-iframe")]
        public IActionResult ProjectScreenshotsIframe(string id)
        {
            return View();
        }

    }
}
