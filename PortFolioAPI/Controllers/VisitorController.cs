using DataAccess.Dto;
using DataAccess.Repositories;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        public VisitorController(IRepository repository, INotificationService iNotificationService)
        {
            _repository = repository;
            _iNotificationService = iNotificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] ViewerDto viewerDto)
        {
            try
            {
                // Process visitor details here
                var totalRecords = _repository.Add(viewerDto);
                    await _iNotificationService.SendNotification($@"
                    👀 Someone visited your Portfolio
                    Real-time visitor alert

                    Location
                    📍 {viewerDto.city}, {viewerDto.country_name}

                    Visit Time
                    🕐 {viewerDto.visit_time}

                    Browser
                    🌐 {viewerDto.browser}

                    OS
                    💻 {viewerDto.operating_system}
                ");

                return Ok(new { status = "Visitor details received", totalRecords = totalRecords });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
