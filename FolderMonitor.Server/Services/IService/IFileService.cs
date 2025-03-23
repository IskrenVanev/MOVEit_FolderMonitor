namespace FolderMonitor.Server.Services.IService
{
    public interface IFileService
    {
        Task<int?> GetUserDefaultFolderIdAsync(string token);
        Task<string> GetFolderContentsUsingCurl(string token, int folderId);
       // void StartMonitoring();
       // void StopMonitoring();
    }
}
