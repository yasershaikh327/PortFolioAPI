using DataAccess.AppSettings;
using DataAccess.Helper;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using PortFolioAPI.DtoModels.Request;
using PortFolioAPI.Models;

namespace PortFolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("public-api")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _iNotificationService;
        private readonly AppSettings _settings;
        private readonly IHelper _helper;
        public NotificationController(INotificationService notificationService, IOptions<AppSettings> options, IHelper helper)
        {
            _iNotificationService = notificationService;
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
                    // Check if cookie exists
                    if (Request.Cookies.ContainsKey("SmsSent"))
                    {
                        return new JsonResult(new { status = "Notification already sent recently" });
                    }

                    // Send SMS notification
                    await _iNotificationService.SendWhatsppMessageByTwilio($"Hello {viewerDto.city}, Good Morning 🌞");

                    // Set temporary cookie (expires in 24 hours)
                    Response.Cookies.Append("SmsSent", "true", new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddHours(24),
                        HttpOnly = true,
                        Secure = true
                    });

                    return new JsonResult(new { status = "Notification sent" });
                }

                return Ok(new { status = "Running on Localhost..." });
            }
            catch (Exception ex)
            {
                _helper.LogError("An error occurred while processing the notification request.", ex);
                throw new Exception(ex.Message);
            }
        }

    }
}
