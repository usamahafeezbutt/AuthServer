
namespace AuthServer.Domain.Common
{
    public class BaseAuditEntity : ICreationAudit, IModificationAudit, ISoftDelete
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public Guid? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

    }
}
