using DataAccess.Helper;
using Microsoft.AspNetCore.Mvc;

namespace PortFolioAPI.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IHelper _helper;
        public ErrorController(IHelper helper)
        {
            _helper = helper;
        }

        [Route("Error/Error")]
        public IActionResult Error(int statusCode = 500)
        {
            try
            {
                Response.StatusCode = statusCode;
                _helper.LogError($"An error occurred while processing the request. Status Code: {statusCode}");
                return View("Error", statusCode);
            }
            catch (Exception ex)
            {
                _helper.LogError("An error occurred while processing the request.", ex);
                throw ex;
            }
        }
    }
}
