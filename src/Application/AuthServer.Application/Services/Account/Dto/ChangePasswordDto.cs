

namespace AuthServer.Application.Services.Account.Dto
{
    public class ChangePasswordDto
    {
        public string Password { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
