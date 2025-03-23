using FolderMonitor.Server.Services;
using FolderMonitor.Server.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace FolderMonitor.Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FileController> _logger;
        private readonly IAuthService _authService;
        private readonly IFileService _fileService;
        private readonly TokenService _tokenService;
        public FileController(ILogger<FileController> logger, HttpClient httpClient, IAuthService authService, IFileService fileService, TokenService tokenService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _authService = authService;
            _fileService = fileService; 
            _tokenService = tokenService;
        }


        [HttpGet("list-folder-contents")]
        public async Task<IActionResult> ListFolderContents()
        {
            try
            {
                // Retrieve the token globally from TokenService
                var token = _tokenService.AccessToken;

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("No valid access token found. Please login first.");
                }

                var defaultFolderId = await _fileService.GetUserDefaultFolderIdAsync(token);

                if (defaultFolderId == null)
                {
                    return NotFound("Default folder ID not found for the user.");
                }

                // Call the method to list the contents using curl
                var filesContent = await _fileService.GetFolderContentsUsingCurl(token, defaultFolderId.Value);

                return Ok(filesContent); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error listing folder contents: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
  
}
