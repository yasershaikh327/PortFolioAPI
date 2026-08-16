using Microsoft.AspNetCore.Mvc;

namespace PortFolioAPI.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Home/Error/{code}")]
        public IActionResult HandleError(int code)
        {
            return code switch
            {
                //404 => View("NotFound"),
                405 => View("MethodNotAllowed"),
                429 => View("TooManyRequests"),
                500 => View("ServerError"),
                502 => View("BadGateway"),
                _ => View("GenericError")
            };
        }
    }
}
