using DataAccess.Helper;
using DataAccess.Model;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PortFolioAPI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepository _repository;
        private readonly IHelper _helper;

        public AdminController(IRepository repository, IHelper helper)
        {
            _repository = repository;
            _helper = helper;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost("/api/Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _repository.Login(login);

                    if (data)
                    {
                        var token = _helper.GenerateJwtToken(login.Email);

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, login.Email)
                        };

                        var identity = new ClaimsIdentity(
                            claims,
                            CookieAuthenticationDefaults.AuthenticationScheme
                        );

                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal
                        );

                        return Ok(new
                        {
                            data = true,
                            token = token,
                            statusCode = 200
                        });
                    }
                }

                return Ok(new
                {
                    data = "Invalid Credentials",
                    statusCode = 400
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ✅ New endpoint to accept token from frontend
        [AllowAnonymous]
        [HttpPost("/Admin/SetToken")]
        public IActionResult SetToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request?.Token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            // Store token in session (or DB/cache if needed)
            HttpContext.Session.SetString("jwtToken", request.Token);

            return Ok(new { message = "Token stored successfully" });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Portfolio()
        {
            // Optional: validate token from session
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return View();
        }
    }

    // DTO for SetToken
    public class TokenRequest
    {
        public string Token { get; set; }
    }
}
