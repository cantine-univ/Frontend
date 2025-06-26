namespace FrontendApp.Models
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty; // Cette propriété est cruciale
        public string Role { get; set; } = string.Empty; 
    }
}