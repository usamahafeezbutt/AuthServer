

namespace AuthServer.Domain.Common
{
    public interface ICreationAudit
    {
        public DateTime Created { get; set; }

        public Guid? CreatedBy { get; set; }
    }
}
