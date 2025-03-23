using FolderMonitor.Server.Helpers;
using FolderMonitor.Server.Services.IService;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace FolderMonitor.Server.Services
{
    public class FileService : IFileService
    {
       // private DirectoryWatcherHelper _directoryWatcher;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthService _authService;
        private readonly ILogger<FileService> _logger;

        public FileService(IAuthService authService, ILogger<FileService> logger, IHttpClientFactory httpClientFactory)
        {
            _authService = authService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            // Initialize the directory watcher
            //_directoryWatcher = new DirectoryWatcherHelper(directoryToWatch);

            // Capture token in a closure
           // _directoryWatcher.OnNewFileCreated += (sender, e) => HandleNewFileCreated(sender, e, "your-token-here");
        }
        public async Task<int?> GetUserDefaultFolderIdAsync(string token)
        {
            var userInfoResult = await _authService.GetUserInfoAsync(token);

            if (string.IsNullOrEmpty(userInfoResult) || userInfoResult.StartsWith("An error occurred"))
            {
                return null;
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(userInfoResult))
                {
                    if (doc.RootElement.TryGetProperty("defaultFolderID", out JsonElement folderIdElement) &&
                        folderIdElement.ValueKind == JsonValueKind.Number)
                    {
                        return folderIdElement.GetInt32();
                    }
                }
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON Parsing Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetFolderContentsUsingCurl(string token, int folderId)
        {
            string curlCommand = $"curl -k -H \"Authorization: Bearer {token}\" \"https://testserver.moveitcloud.com/api/v1/folders/{folderId}/files\"";

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
                _logger.LogError($"Error executing curl command: {ex.Message}");
                return $"An error occurred while executing the curl command: {ex.Message}";
            }
        }

        //private void HandleNewFileCreated(object sender, FileSystemEventArgs e, string token)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"New file detected: {e.FullPath}");

        //        // Upload the file to MOVEit Transfer
        //        UploadFileToMoveitTransfer(e.FullPath, token).Wait();  // Call async method synchronously
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error handling new file creation: {ex.Message}");
        //    }
        //}

        //private async Task UploadFileToMoveitTransfer(string filePath, string token)
        //{
        //    try
        //    {
        //        var folderId = await GetUserDefaultFolderIdAsync(token);

        //        if (!folderId.HasValue)
        //        {
        //            _logger.LogError("Failed to retrieve the default folder ID.");
        //            return;
        //        }

        //        var apiUrl = $"https://testserver.moveitcloud.com/api/v1/folders/{folderId}/files";

        //        using var client = _httpClientFactory.CreateClient();

        //        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}"); //TODO: This will not work fix it

        //        using var formContent = new MultipartFormDataContent();
        //        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //        var fileContent = new StreamContent(fileStream);
        //        formContent.Add(fileContent, "file", Path.GetFileName(filePath));

        //        // Send the POST request to upload the file
        //        var response = await client.PostAsync(apiUrl, formContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            _logger.LogInformation($"File '{filePath}' uploaded successfully to folder {folderId}.");
        //        }
        //        else
        //        {
        //            _logger.LogError($"Failed to upload file '{filePath}' to folder {folderId}. Status code: {response.StatusCode}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error uploading file '{filePath}': {ex.Message}");
        //    }
        //}

        //public void StartMonitoring()
        //{
        //    Console.WriteLine("Started monitoring the folder...");
        //}

        //public void StopMonitoring()
        //{
        //    Console.WriteLine("Stopped monitoring the folder.");
        //}

        
    }
}
