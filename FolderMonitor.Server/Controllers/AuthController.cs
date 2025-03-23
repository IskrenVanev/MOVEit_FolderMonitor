using FolderMonitor.Server.DTO;
using FolderMonitor.Server.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace FolderMonitor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly TokenService _tokenService;

        public AuthController(ILogger<AuthController> logger, AuthService authService, TokenService tokenService)
        {
            _logger = logger;
            _authService = authService;
            _tokenService = tokenService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var tokenJson = await _authService.GetAccessTokenAsync(loginRequest.Username, loginRequest.Password);

            if (string.IsNullOrEmpty(tokenJson))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Deserialize the response and extract only the access token
            var tokenObject = JsonSerializer.Deserialize<TokenResponseDTO>(tokenJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenObject?.AccessToken == null)
            {
                return Unauthorized("Invalid token response.");
            }

            // Store only the access token
            _tokenService.AccessToken = tokenObject.AccessToken;

            return Ok(tokenObject.AccessToken);
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckAuthentication()
        {
            // Retrieve the token globally from TokenService
            var token = _tokenService.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("No valid access token found. Please login first.");
            }

            var result = await _authService.GetUserInfoAsync(token);

            if (result.StartsWith("An error occurred"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }

}
