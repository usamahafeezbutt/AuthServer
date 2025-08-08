using AuthServer.Application.Common.Constants.Database;
using AuthServer.Application.Common.Constants.Files.EmailTemplates;
using AuthServer.Application.Common.Interfaces.ContentFiles;
using AuthServer.Application.Common.Interfaces.Email;
using AuthServer.Application.Common.Interfaces.Identity;
using AuthServer.Application.Common.Interfaces.Repositories;
using AuthServer.Application.Common.Interfaces.Services;
using AuthServer.Application.Common.Models.Identity;
using AuthServer.Application.Common.Models.Options;
using AuthServer.Application.Common.Models.Responses;
using AuthServer.Application.Models.Identity;
using AuthServer.Application.Services.Account.Dto;
using AuthServer.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AuthServer.Application.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly IFileService _contentFileService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;
        private readonly IGenericRepository<ApplicationToken> _applicationTokenRepository;
        private readonly JwtSettings _jwtSettings;
        public AccountService(
            IIdentityService identityService,
            ITokenService tokenService,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<AccountService> logger,
            IFileService contentFileService,
            IEmailService emailService,
            IMemoryCache memoryCache,
            IGenericRepository<ApplicationToken> applicationTokenRepository,
            IOptions<JwtSettings> jwtSettings)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
            _contentFileService = contentFileService;
            _emailService = emailService;
            _memoryCache = memoryCache;
            _applicationTokenRepository = applicationTokenRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ResultResponse<AuthResponse>> AuthenticateAsync(AuthRequestDto request)
        {
            _logger.LogInformation($"Started Execution of method {nameof(AuthenticateAsync)}");

            try
            {
                var user = await _identityService.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new ResultResponse<AuthResponse>(false, "No user found.", null!);
                }

                var passwordCheck = await _identityService.CheckPasswordAsync(user, request.Password);

                if (!passwordCheck)
                    return new ResultResponse<AuthResponse>(false, "Invalid username or password.", null!);

                ResultResponse<AuthResponse> token = await ManageApplicationToken(user);

                _logger.LogInformation($"Completed Execution of method {nameof(AuthenticateAsync)}");

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error occured while Execution" +
                    $" of method {nameof(AuthenticateAsync)} {ex}");
                return null!;
            }
        }

        public async Task<BaseResponse> UpdateEmail(UpdateEmailDto updateEmailDto)
        {
            var user = await _identityService.FindByIdAsync(_currentUserService.UserId.ToString());
         
            if (user == null)
            {
                return new BaseResponse(false, "No user found");
            }

            user.Email = updateEmailDto.Email;

            var result = await _identityService.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                return new BaseResponse(false, "Email not changed, please try again later.");
            }

            return new BaseResponse(true, "Email changed successfully.");
        }

        public async Task<BaseResponse> UpdateName(UpdateNameDto updateNameDto)
        {
            var user = await _identityService.FindByIdAsync(_currentUserService.UserId.ToString());
         
            if (user == null)
            {
                return new BaseResponse(false, "No user found");
            }

            user.Name = updateNameDto.Name;

            var result = await _identityService.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                return new BaseResponse(false, "Name not changed, please try again later.");
            }

            return new BaseResponse(true, "Name changed successfully.");
        }

        public async Task<BaseResponse> UpdatePhone(UpdatePhoneDto updatePhoneDto)
        {
            var user = await _identityService.FindByIdAsync(_currentUserService.UserId.ToString());
         
            if (user == null)
            {
                return new BaseResponse(false, "No user found");
            }

            user.PhoneNumber = updatePhoneDto.Phone;

            var result = await _identityService.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                return new BaseResponse(false, "Phone not changed, please try again later.");
            }

            return new BaseResponse(true, "Phone changed successfully.");
        }

        public async Task<BaseResponse> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _identityService.FindByIdAsync(_currentUserService.UserId.ToString());
            if (user == null)
            {
                return new BaseResponse(false, "No user found");
            }

            var checkPasswordResult = await _identityService.CheckPasswordAsync(user, changePasswordDto.Password);
            if (!checkPasswordResult)
                return new BaseResponse(false, "Email or password is incorrect");

            var result = await _identityService.ChangePasswordAsync(user, changePasswordDto.Password, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                return new BaseResponse(false, "No user found");
            }

            return new BaseResponse(false, "Password changed successfully.");
        }

        public async Task<ResultResponse<AuthResponse>> RegisterAsync(RegistrationRequestDto request)
        {
            var user = _mapper.Map<ApplicationUser>(request);
            user.Status = Domain.Enums.AccountStatus.Active;
            var result = await _identityService.CreateUserAsync(user, request.Password);
            return await HandleUserOnbaording(request.Role, user, result);
        }

        public async Task<BaseResponse> CreateNewPassword(CreateNewPasswordDto createNewPasswordDto)
        {
            var user = await _identityService.FindByEmailAsync(createNewPasswordDto.Email);

            if (user is null)
                return new BaseResponse(false, "The email does not exist, please try with a valid email account.");

            var result = await _identityService.AddNewPassword(user, createNewPasswordDto.Password);

            if (!result.Succeeded)
                return new BaseResponse(false, "Password not changed, please try again or reach out to customer support");

            return new BaseResponse(true, "New password added successfully");
        }

        public async Task<BaseResponse> VerifyForgotPasswordOTP(VerifyOTPDto request)
        {
            await Task.FromResult(_memoryCache.TryGetValue(request.OTP.Trim(), out var otpValue));

            if (otpValue == null)
                return new BaseResponse(false, "This is not found, which means it even invlaid or expired.");

            _memoryCache.Remove(request.OTP);

            return new BaseResponse(true, "OTP exists.");
        }

        public async Task<BaseResponse> SendForgotPasswordEmail(ForgotPassword forgotPassword)
        {
            var user = await _identityService.FindByEmailAsync(forgotPassword.Email);

            if (user is null)
                return new BaseResponse(false, "The email does not exist, please try with a valid email account.");

            var otp = new Random().Next(100000, 1000000).ToString();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            _memoryCache.Set(otp, cacheEntryOptions);

            EmailOptions emailOptions = new()
            {
                Subject = "Forgot Password Request at AuthServer",
                ToEmails = [forgotPassword.Email],
                Body = PrepareForgotPasswordEmailBody(user.Name, otp)
            };

            await _emailService.SendEmail(emailOptions);

            return new BaseResponse(true, "Forgot password email sent successfully.");
        }

        private string PrepareForgotPasswordEmailBody(string name, string otp)
        {
            string templateContent = _contentFileService
                .GetEmailTemplateFile(ForgotPasswordEmailTemplateConstants.EmailTemplateName);

            templateContent = templateContent.Replace(ForgotPasswordEmailTemplateConstants.CustomerName, name);
            templateContent = templateContent.Replace(ForgotPasswordEmailTemplateConstants.OTP, otp);

            return templateContent;
        }

        public async Task<ResultResponse<List<AccountDetailDto>>> GetAccountDetails(
            GetPagedAccountDetailDto getPagedAccountDetailDto)
        {
            var users = await _identityService.GetAllUsersAsync(getPagedAccountDetailDto);

            if (users is null)
            {
                return new ResultResponse<List<AccountDetailDto>>(
                    false,
                    "No user found, please try again.",
                    null!);
            }

            return new ResultResponse<List<AccountDetailDto>>(
                    true,
                    "User found successfully.",
                    users.Result!);
        }

        public async Task<ResultResponse<AccountInfoDto>> GetAccountInfo()
        {
            var result = await _identityService.FindByIdAsync(_currentUserService.UserId.ToString());

            if (result is null)
            {
                return new ResultResponse<AccountInfoDto>(
                    false,
                    "No user found, please try again.",
                    null!);
            }

            return new ResultResponse<AccountInfoDto>(
                    true,
                    "User found successfully.",
                    _mapper.Map<AccountInfoDto>(result));
        }

        public async Task<BaseResponse> UpdateAccountStatus(
            UpdateAccountStatusDto accountStatusDto)
        {
            var user = await _identityService.FindByIdAsync(accountStatusDto.AccountId);

            if (user is null)
            {
                return new ResultResponse<BaseResponse>(
                    false,
                    "No user found, please try again.",
                    null!);
            }

            user.Status = accountStatusDto.Status;

            var result = await _identityService.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                return new ResultResponse<BaseResponse>(
                    false,
                    result.Errors?.FirstOrDefault()?.Description!,
                    null!);
            }

            return new ResultResponse<BaseResponse>(
                    true,
                    "User found successfully.",
                    new BaseResponse(true, "Account Status updated successfully."));
        }

        public async Task<BaseResponse> UpdateAccountDetails(
            AccountDetailDto accountStatusDto)
        {
            var user = await _identityService.FindByEmailAsync(accountStatusDto.Email);

            if (user is null)
            {
                return new ResultResponse<BaseResponse>(
                    false,
                    "No user found, please try again.",
                    null!);
            }

            user.UserName = accountStatusDto.UserName;
            user.PhoneNumber = accountStatusDto.PhoneNumber;
            user.Email = accountStatusDto.Email;
            user.Name = accountStatusDto.Name;
            user.Status = accountStatusDto.Status;

            var result = await _identityService.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                return new ResultResponse<BaseResponse>(
                    false,
                    result.Errors?.FirstOrDefault()?.Description!,
                    null!);
            }

            var existingRole = await _identityService.GetUserRoles(user);
                
            if(existingRole.First() != accountStatusDto.Role)
            {
                var isRoleRemoved = await _identityService.RemoveRoleAsync(user, existingRole.First());

                var roleResult = await _identityService.AssignRoleAsync(user, accountStatusDto.Role);

                if (!roleResult.Succeeded)
                {
                    return new ResultResponse<AuthResponse>(
                        false,
                        result.Errors.Any() ? result.Errors?.FirstOrDefault()?.ToString()! : "User Account not created, please try again."!,
                    null!);
                }
            }
            
            return new ResultResponse<BaseResponse>(
                    true,
                    "User found successfully.",
                    new BaseResponse(true, "Account Status updated successfully."));
        }

        public async Task<ResultResponse<AccountInfoDto>> UpdateAccountInfo(AccountInfoDto accountInfoDto)
        {
            var user = await _identityService.FindByIdAsync(_currentUserService.UserId.ToString());

            if (user is null)
            {
                return new ResultResponse<AccountInfoDto>(
                    false,
                    "No user found, please try again.",
                    null!);
            }

            user.Name = accountInfoDto.Name;
            user.Email = accountInfoDto.Email;
            user.UserName = accountInfoDto.UserName;
            user.PhoneNumber = accountInfoDto.PhoneNumber;

            var result = await _identityService.UpdateUserAsync(user);

            if (!result.Succeeded)
            {
                return new ResultResponse<AccountInfoDto>(
                    false,
                    result.Errors?.FirstOrDefault()?.Description!,
                    null!);
            }

            return new ResultResponse<AccountInfoDto>(
                    true,
                    "User found successfully.",
                    accountInfoDto);
        }

        public async Task<ResultResponse<ExternalLoginAuthResponse>> Callback(AuthenticateResult result)
        {
            var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _identityService.FindByEmailAsync(email!);

            if (user != null)
            {
                var token = await _tokenService.GenerateUserToken(user);

                _logger.LogInformation($"Completed Execution of method {nameof(AuthenticateAsync)}");

                return new ResultResponse<ExternalLoginAuthResponse>(
                    true,
                    "Login Successful",
                    new ExternalLoginAuthResponse
                    {
                        Type = "login",
                        Token = token.Result?.Token!,
                        RefreshToken = token.Result!.RefreshToken,
                        Email = token.Result!.Email,
                        Expiry = token.Result.Expiry
                    }); 
            }

            var name = result?.Principal?.FindFirst(ClaimTypes.Name)?.Value;
            var providerKey = result?.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var provider = result!.Properties!.Items.ContainsKey(".AuthScheme")
                ? result.Properties.Items[".AuthScheme"] : "Unknown";

            var accessToken = result.Properties.GetTokenValue("access_token");
            var refreshToken = result.Properties.GetTokenValue("refresh_token");
            var expiresUtc = DateTime.UtcNow.AddHours(1);
            var refreshTokenExpires = DateTime.UtcNow.AddDays(30);

            user = new ApplicationUser
            {
                Email = email,
                Name = string.IsNullOrEmpty(name) ? name! : email!.Split("@").First(),
                UserName = email,
                Status = Domain.Enums.AccountStatus.Active,
                Provider = provider,
                ProviderKey = providerKey,
                ApplicationToken = new ApplicationToken
                {
                    ExternalAccessToken = accessToken,
                    ExternalRefreshToken = refreshToken,
                    ExternalTokenExpiresAt = expiresUtc,
                    ExternalRefreshTokenExpiresAt = refreshTokenExpires,
                }
            };


            var userResult = await _identityService.CreateUserAsync(user);

            var onboardingResult = await HandleUserOnbaording(
                ApplicationRoleConstants.UserRole,
                user,
                userResult);

            return new ResultResponse<ExternalLoginAuthResponse>(
                    true,
                    "Login Successful",
                    new ExternalLoginAuthResponse
                    {
                        Type = "register",
                        RefreshToken = onboardingResult.Result!.RefreshToken,
                        Token = onboardingResult.Result?.Token!,
                        Email = onboardingResult.Result!.Email,
                        Expiry = onboardingResult.Result.Expiry
                    });
        }

        public async Task<ResultResponse<AuthResponse>> RefreshToken(RefreshTokenRequestDto requestDto)
        {
            var applicationToken = await _applicationTokenRepository
                .TableNoTracking
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.RefreshToken == requestDto.RefreshToken);

            if (applicationToken is null)
                return new ResultResponse<AuthResponse>(false, "Refresh token not found.", null!);

            if (applicationToken.RefreshTokenExpiresAt < DateTime.UtcNow)
                return new ResultResponse<AuthResponse>(false, "Refresh token is expired.", null!);

            var token = await _tokenService.GenerateUserToken(applicationToken.User!);

            token.Result!.RefreshToken = applicationToken.RefreshToken!.Value;

            return token;
        }

        public async Task<BaseResponse> DeleteRefreshToken(RefreshTokenRequestDto requestDto)
        {
            try
            {
                var applicationToken = await _applicationTokenRepository
                        .TableNoTracking
                        .Include(x => x.User)
                        .FirstOrDefaultAsync(x => x.RefreshToken == requestDto.RefreshToken);

                if (applicationToken is null)
                    return new ResultResponse<AuthResponse>(false, "Refresh token not found", null!);

                await _applicationTokenRepository.Delete(applicationToken);

                return new BaseResponse(true, "Refresh token deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while deleting the refresh token of user {0}" +
                    " with exception: {1}", _currentUserService.UserId, ex);

                return new BaseResponse(false, "An error occured while deleting the refresh token.");
            }
        }

        private async Task<ResultResponse<AuthResponse>> ManageApplicationToken(ApplicationUser user)
        {
            var token = await _tokenService.GenerateUserToken(user);

            var applicationToken = await _applicationTokenRepository
            .TableNoTracking
            .FirstOrDefaultAsync(x => x.UserId == user.Id) ?? new ApplicationToken();

            applicationToken!.UserId = user.Id;
            applicationToken!.AccessToken = token.Result!.Token;
            applicationToken.RefreshToken = applicationToken.Id == Guid.Empty
                ? token.Result!.RefreshToken : applicationToken.RefreshToken;
            applicationToken.TokenExpiresAt = token.Result.Expiry;
            applicationToken.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiry);

            if (applicationToken.Id == Guid.Empty)
                await _applicationTokenRepository.Add(applicationToken);
            else
                await _applicationTokenRepository.Update(applicationToken);

            if (applicationToken is not null)
                token.Result.RefreshToken = applicationToken.RefreshToken!.Value;
            return token;
        }

        private async Task<ResultResponse<AuthResponse>> HandleUserOnbaording(
            string role,
            ApplicationUser user,
            IdentityResult result)
        {
            if (!result.Succeeded)
            {
                return new ResultResponse<AuthResponse>(
                    false,
                    result.Errors.Any() ? result.Errors?.FirstOrDefault()?.Description.ToString()! : "User Account not created, please try again."!,
                    null!);
            }

            var identityUser = await _identityService.FindByEmailAsync(user.Email!);

            var roleResult = await _identityService.AssignRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                // Handle role assignment errors (you might want to remove the user if this fails)
                return new ResultResponse<AuthResponse>(
                    false,
                    result.Errors.Any() ? result.Errors?.FirstOrDefault()?.ToString()! : "User Account not created, please try again."!,
                null!);
            }

            EmailOptions emailOptions = new()
            {
                Subject = $"Account Confirmation At AuthServer",
                ToEmails = [user.Email!],
                Body = PrepareSupportRequestEmailBody(user.Name)
            };

            await _emailService.SendEmail(emailOptions);

            return await _tokenService.GenerateUserToken(identityUser);
        }

        private string PrepareSupportRequestEmailBody(string name)
        {
            string templateContent = _contentFileService.GetEmailTemplateFile(AccountConfirmationEmailTemplateConstants.EmailTemplateName);

            templateContent = templateContent.Replace(AccountConfirmationEmailTemplateConstants.CustomerName, name);

            return templateContent;
        }
    }
}
