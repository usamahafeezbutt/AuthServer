using AuthServer.Domain.Common;

namespace AuthServer.Domain.Entities
{
    public class ApplicationToken : BaseAuditEntity
    {
        public string? AccessToken { get; set; }
        public Guid? RefreshToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public string? ExternalAccessToken { get; set; }
        public string? ExternalRefreshToken { get; set; }
        public DateTime? ExternalTokenExpiresAt { get; set; }
        public DateTime? ExternalRefreshTokenExpiresAt { get; set; }
        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
