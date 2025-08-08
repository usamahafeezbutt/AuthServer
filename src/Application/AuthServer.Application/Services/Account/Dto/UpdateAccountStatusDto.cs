
using AuthServer.Domain.Enums;

namespace AuthServer.Application.Services.Account.Dto
{
    public class UpdateAccountStatusDto
    {
        public string AccountId { get; set; } = null!;
        public AccountStatus Status { get; set; } 
    }
}
