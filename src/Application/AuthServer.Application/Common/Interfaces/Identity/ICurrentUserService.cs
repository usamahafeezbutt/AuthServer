
namespace AuthServer.Application.Common.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
