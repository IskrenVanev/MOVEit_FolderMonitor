using FolderMonitor.Server.DTO;
using FolderMonitor.Server.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FolderMonitor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var accessToken = await _authService.GetAccessTokenAsync(loginRequest.Username, loginRequest.Password);

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new { AccessToken = accessToken });
        }

    }

}
