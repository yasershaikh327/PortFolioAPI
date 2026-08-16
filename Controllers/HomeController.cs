using Microsoft.AspNetCore.Mvc;
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

            // Send notification to ntfy public server, topic: yasershaikhportfoliovisitors327
            var response = await client.PostAsync("https://ntfy.sh/yasershaikhportfoliovisitors327", message);

            return Json(new { status = response.StatusCode });
        }
    }
}
