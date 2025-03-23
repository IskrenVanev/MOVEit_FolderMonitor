using FolderMonitor.Server.DTO;
using FolderMonitor.Server.Services.IService;
using System.Diagnostics;
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
            var formData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.PostAsync("https://testserver.moveitcloud.com/api/v1/token", content);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody; 
        }

        public async Task<string> GetUserInfoAsync(string token)
        {
            string curlCommand = $"curl -k -H \"Authorization: Bearer {token}\" \"https://testserver.moveitcloud.com/api/v1/users/self\"";

            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = "curl",
                Arguments = curlCommand,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(processStartInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = await reader.ReadToEndAsync();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred while executing the curl command: {ex.Message}";
            }
        }

    }
}
