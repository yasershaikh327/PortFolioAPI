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
        private readonly INotificationService _iNotificationService;
        private readonly AppSettings _settings;
        private readonly IHelper _helper;
        public VisitorController(IRepository repository, INotificationService iNotificationService, IOptions<AppSettings> options, IHelper helper)
        {
            _repository = repository;
            _iNotificationService = iNotificationService;
            _settings = options.Value;
            _helper = helper;
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

                        await _iNotificationService.SendNotification(
                            $"👀 Visitor Alert: Location 📍 {viewerDto.city}, {viewerDto.country_name}; " +
                            $"Time 🕐 {VisitorDate.ToString("dd/MM/yyyy hh:mm tt")}; " +
                            $"Browser 🌐 {viewerDto.browser}; OS 💻 {viewerDto.operating_system}"
                        );

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

                return Ok(new
                {
                    DatabaseStatus = $"{ex.Message} | Inner: {innerMessage} | Deeper: {deeperMessage}"
                });
             }
            catch (Exception ex)
            {
                _helper.LogError("An error occurred while processing the visitor request.", ex);
                return Ok(new { MethodStatus = ex.Message });
            }
        }
    }
}
