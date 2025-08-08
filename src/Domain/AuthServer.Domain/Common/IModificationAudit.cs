namespace AuthServer.Domain.Common
{
    public interface IModificationAudit
    {
        public DateTime? LastModified { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}
