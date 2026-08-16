using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PortFolioAPI.Models;

namespace PortFolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("public-api")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _iNotificationService;
        public NotificationController(INotificationService notificationService)
        {
            _iNotificationService = notificationService;
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
                throw new Exception(ex.Message);
            }
        }
    }
}
