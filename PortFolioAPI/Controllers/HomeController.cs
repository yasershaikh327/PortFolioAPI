using DataAccess.AppSettings;
using DataAccess.Repositories;
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
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }   

        public IActionResult Index()
        {
            return View();
        }

        [Route("Visitors")]
        public IActionResult Visitors()
        {
            var data = _repository.GetVisitors();
            return new JsonResult(new { data = data, message = "Visitors data" });
        }

        [Route("project-screenshots-iframe")]
        public IActionResult ProjectScreenshotsIframe()
        {
            return View();
        }

    }
}
