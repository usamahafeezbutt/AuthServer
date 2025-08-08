using AuthServer.Domain.Enums;

namespace AuthServer.Application.Services.Account.Dto
{
    public class AccountDetailDto : AccountInfoDto
    {
        public AccountStatus Status { get; set; }
        public string Role { get; set; } = null!;
    }
}
