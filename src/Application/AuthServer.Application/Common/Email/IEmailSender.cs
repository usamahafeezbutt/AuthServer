
using AuthServer.Application.Common.Models.Options;
using AuthServer.Application.Common.Models.Responses;

namespace AuthServer.Application.Common.Interfaces.Email
{
    public interface IEmailService
    {
        Task<BaseResponse> SendEmail(EmailOptions emailOptions);
    }
}
