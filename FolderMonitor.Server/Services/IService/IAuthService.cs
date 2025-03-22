namespace FolderMonitor.Server.Services.IService
{
    public interface IAuthService
    {
        Task<string> GetAccessTokenAsync(string username, string password);
    }
}
