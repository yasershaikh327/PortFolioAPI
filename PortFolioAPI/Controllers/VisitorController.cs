using DataAccess.Dto;
using DataAccess.Repositories;
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
        public VisitorController(IRepository repository)
        {
            _repository = repository;   
        }

        [HttpPost]
        public IActionResult Index([FromBody] ViewerDto viewerDto)
        {
            try
            {
                // Process visitor details here
                var totalRecords = _repository.Add(viewerDto);
                return Ok(new { status = "Visitor details received", totalRecords = totalRecords });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
