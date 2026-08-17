using DataAccess.AppSettings;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace PortFolioAPI.Controllers
{
    [EnableRateLimiting("public-api")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
