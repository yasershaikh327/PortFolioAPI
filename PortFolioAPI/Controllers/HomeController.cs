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
        public IActionResult NotFoundPage()
        {
            // Simulate a scenario where the requested resource does not exist.
            bool resourceExists = false;
            if (!resourceExists)
            {
                // Returns 404 Not Found to trigger the custom error page.
                return NotFound();
            }
            return View();
        }

    }
}
