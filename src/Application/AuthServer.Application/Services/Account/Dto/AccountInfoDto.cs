using AuthServer.Application.Common.Dtos.Common;

namespace AuthServer.Application.Services.Account.Dto
{
    public class AccountInfoDto : BaseEntityDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
