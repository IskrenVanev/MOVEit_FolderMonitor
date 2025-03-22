namespace FolderMonitor.Server.DTO
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}
