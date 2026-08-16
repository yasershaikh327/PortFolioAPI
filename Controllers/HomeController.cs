using Microsoft.AspNetCore.Mvc;
using PortFolioAPI.Models;
using System.Diagnostics;

namespace PortFolioAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Health()
        {
            return Json(new { status = "ok" });
        }
    }

}
