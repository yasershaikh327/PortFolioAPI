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


        [HttpGet("/project-screenshots-iframe")]
        public IActionResult ProjectScreenshotsIframe(string id)
        {
            return View();
        }

        // Simulate a 404 Not Found error page.
        [HttpGet("/Home/page-not-found-error")]
        public IActionResult PageNotFoundError()
        {
            return View();
        }

    }
}
