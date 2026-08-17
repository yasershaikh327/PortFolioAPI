using DataAccess.AppSettings;
using DataAccess.Helper;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
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
                // You can include userDetails info in the message if you want
                await _iNotificationService.SendNotification($"Hello {viewerDto.city}, Good Morning 🌞");
                return new JsonResult(new { status = "Notification sent" });
            }
            catch (Exception ex)
            {
                _helper.LogError("An error occurred while processing the notification request.", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
