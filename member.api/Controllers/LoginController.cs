using System.Threading.Tasks;
using member.api.Shared.Facades;
using member.api.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace member.api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IAuthenFacades _facade;
        public LoginController(ILogger<LoginController> logger, IAuthenFacades facade)
        {
            _logger = logger;
            _facade = facade;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromHeader(Name = "system-name")] string systemName, LoginRequest user)
        {
            _logger.LogInformation($"Login User {user.Username}");

            var resp = await _facade.Login(user, systemName);

            if (!string.IsNullOrEmpty(resp.Token))
            {
                return Ok(resp);
            }

            return Unauthorized(resp);
        }
    }
}