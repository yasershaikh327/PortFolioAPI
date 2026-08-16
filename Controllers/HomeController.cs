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

        public IActionResult Health()
        {
            return Json(new { status = "ok" });
        }

        [Route("notify")]
        public async Task<IActionResult> Notify()
        {
            using var client = new HttpClient();

            var message = new StringContent("Good Morning 🌞", Encoding.UTF8);
            message.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            var response = await client.PostAsync("https://ntfy.sh/yasershaikhportfoliovisitors327", message);
            var responseText = await response.Content.ReadAsStringAsync();

            return Json(new
            {
                status = response.StatusCode,
                response = responseText
            });
        }
    }
}
