namespace FolderMonitor.Server.Services.IService
{
    public interface IAuthService
    {
        Task<string> GetAccessTokenAsync(string username, string password);
        Task<string> GetUserInfoAsync(string token);
    }
}
