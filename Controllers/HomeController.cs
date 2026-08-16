using Microsoft.AspNetCore.Mvc;
using PortFolioAPI.Models;
using System.Diagnostics;
using System.Text;

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

        [Route("notify")]
        public async Task<IActionResult> Notify()
        {
            using var client = new HttpClient();
            var message = new StringContent("Good Morning 🌞", Encoding.UTF8, "text/plain");

            // "mytopic" is any name you choose
            var response = await client.PostAsync("https://ntfy.sh/mytopic", message);

            return Json(new { status = response.StatusCode });
        }

    }

}
