using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using member.api.Shared.Constant;
using member.api.Shared.Facades;
using member.api.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace member.api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthenController : ControllerBase
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly IAuthenFacades _facade;
        public AuthenController(ILogger<AuthenController> logger, IAuthenFacades facade)
        {
            _logger = logger;
            _facade = facade;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<RegisterActivate>> Register([FromHeader(Name = "x-system-key")] string SystemId, [FromBody] Register dto)
        {
            _logger.LogInformation("System: {@SystemId}", new { SystemId });
            var resp = await _facade.Register(dto);
            if (resp.Message == Message.Success)
            {
                return Ok(resp);
            }
            return BadRequest();
        }
    }
}