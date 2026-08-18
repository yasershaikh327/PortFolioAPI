using DataAccess.AppSettings;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
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
        [Route("project-screenshots-iframe")]
        public IActionResult ProjectScreenshotsIframe()
        {
            return View();
        }

    }
}
