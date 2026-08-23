using DataAccess.Helper;
using DataAccess.Model;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Authorize]
        [HttpGet]
        public IActionResult Portfolio()
        {
            return View();
        }
    }
}
