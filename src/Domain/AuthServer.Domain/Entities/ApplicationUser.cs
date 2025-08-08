using AuthServer.Domain.Common;
using AuthServer.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, ICreationAudit, IModificationAudit
    {
        public string Name { get; set; } = null!;
        public AccountStatus Status { get; set; }
        public string? Provider { get; set; }
        public string? ProviderKey { get; set; }
        public DateTime Created { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public Guid? ApplicaionTokedId { get; set; }
        public ApplicationToken? ApplicationToken { get; set; } = null!;
    }
}