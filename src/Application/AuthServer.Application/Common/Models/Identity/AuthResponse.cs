
namespace AuthServer.Application.Common.Models.Identity
{
    public class AuthResponse
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
