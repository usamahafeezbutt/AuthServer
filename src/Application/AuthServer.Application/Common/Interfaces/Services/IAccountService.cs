using AuthServer.Application.Common.Models.Identity;
using AuthServer.Application.Services.Account.Dto;
using AuthServer.Application.Common.Models.Responses;
using Microsoft.AspNetCore.Authentication;

namespace AuthServer.Application.Common.Interfaces.Services
{
    public interface IAccountService
    {
        Task<ResultResponse<AuthResponse>> RegisterAsync(RegistrationRequestDto request);
        Task<ResultResponse<AuthResponse>> AuthenticateAsync(AuthRequestDto request);
        Task<BaseResponse> ChangePassword(ChangePasswordDto changePasswordDto);
        Task<ResultResponse<AccountInfoDto>> GetAccountInfo();
        Task<ResultResponse<AccountInfoDto>> UpdateAccountInfo(AccountInfoDto accountInfoDto);
        Task<ResultResponse<List<AccountDetailDto>>> GetAccountDetails(
            GetPagedAccountDetailDto getPagedAccountDetailDto);
        Task<BaseResponse> UpdateAccountStatus(
            UpdateAccountStatusDto accountStatusDto);
        Task<BaseResponse> UpdateEmail(UpdateEmailDto updateEmailDto);
        Task<BaseResponse> UpdateName(UpdateNameDto updateNameDto);
        Task<BaseResponse> UpdatePhone(UpdatePhoneDto updatePhoneDto);
        Task<BaseResponse> CreateNewPassword(CreateNewPasswordDto createNewPasswordDto);
        Task<BaseResponse> SendForgotPasswordEmail(ForgotPassword forgotPassword);
        Task<BaseResponse> VerifyForgotPasswordOTP(VerifyOTPDto otp);
        Task<ResultResponse<ExternalLoginAuthResponse>> Callback(AuthenticateResult result);
        Task<BaseResponse> UpdateAccountDetails(
            AccountDetailDto accountStatusDto);
        Task<ResultResponse<AuthResponse>> RefreshToken(RefreshTokenRequestDto requestDto);
        Task<BaseResponse> DeleteRefreshToken(RefreshTokenRequestDto requestDto);
    }
}
