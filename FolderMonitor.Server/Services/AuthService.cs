using FolderMonitor.Server.DTO;
using FolderMonitor.Server.Services.IService;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FolderMonitor.Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync(string username, string password)
        {
            // Prepare the data to send in the x-www-form-urlencoded format
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password }
            };

            // Content for application/x-www-form-urlencoded
            var content = new FormUrlEncodedContent(formData);
            // Set the headers
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


            // Set the Accept header to application/json (response format)
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.PostAsync("https://testserver.moveitcloud.com/api/v1/token", content);

            if (!response.IsSuccessStatusCode)
            {
                // Log the error or handle the failed response
                return null; // Could be Unauthorized or another error
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;  // Return the JSON body with the token
        }
    }
}
