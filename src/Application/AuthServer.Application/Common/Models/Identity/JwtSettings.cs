
namespace AuthServer.Application.Models.Identity
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public int Expiry { get; set; }
        public int RefreshTokenExpiry { get; set; }
    }
}