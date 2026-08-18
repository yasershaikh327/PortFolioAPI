using DataAccess.AppSettings;
using DataAccess.Dto;
using DataAccess.Helper;
using DataAccess.Repositories;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PortFolioAPI.Models;

namespace PortFolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("public-api")]
    public class VisitorController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly AppSettings _settings;
        private readonly IHelper _helper;
        public VisitorController(IRepository repository, INotificationService iNotificationService, IOptions<AppSettings> options, IHelper helper)
        {
            _repository = repository;
            _settings = options.Value;
            _helper = helper;
        }

        [Route("sum")]
        public async Task<IActionResult> Sum()
        {
            try
            {
                int x = 8; int y = 0;
                return Ok(x / y);
            }
            catch (Exception ex)
            {
                _helper.LogError("An error occurred while processing the sum request.", ex);
                Console.Error.WriteLine("Invalid Divided Exception");

                if (StatusCodes.Status500InternalServerError == 500)
                {
                    return Redirect("/Error/Error");
                }
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] ViewerDto viewerDto)
        {
            try
            {
                if (_settings.ISPROD == "YES")
                {
                    if (ModelState.IsValid)
                    {
                        var VisitorDate = DateTime.UtcNow;

                        // Process visitor details here
                        var totalRecords = _repository.Add(viewerDto);
                        return Ok(new { status = "Thank You for Visiting!!!" });

                    }
                    return Ok(new { status = "Something Went Wrong" });
                }
                return Ok(new { status = "Running on Localhost..." });
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message;
                var deeperMessage = ex.InnerException?.InnerException?.Message;

                _helper.LogError("Visitor request failed.", ex);
                Console.Error.WriteLine(ex.ToString());
                Console.Error.WriteLine(innerMessage.ToString());
                Console.Error.WriteLine(deeperMessage.ToString());

                if (StatusCodes.Status500InternalServerError == 500)
                {
                    return Redirect("/Error/Error");
                }

                //return Ok(new
                //{
                //    DatabaseStatus = $"{ex.Message} | Inner: {innerMessage} | Deeper: {deeperMessage}"
                //});
             }
            catch (Exception ex)
            {
                _helper.LogError("An error occurred while processing the visitor request.", ex);
                Console.Error.WriteLine(ex.ToString());

                //return StatusCode(StatusCodes.Status500InternalServerError, new
                //{
                //    statusCode = 500,
                //    message = "An internal server error occurred.",
                //    MethodStatus = ex.Message
                //});
                if (StatusCodes.Status500InternalServerError == 500)
                {
                    return Redirect("/Error/Error");
                }
            }
        }
    }
}
