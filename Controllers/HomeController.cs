using Microsoft.AspNetCore.Mvc;
using PortFolioAPI.Models;
using System.Diagnostics;

namespace PortFolioAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { message = "Hello from MVC on Vercel" });
        }

        public IActionResult Health()
        {
            return Json(new { status = "ok" });
        }
    }

}
