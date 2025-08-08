using AuthServer.Application.Common.Interfaces.Services;
using AuthServer.Application.Common.Models.Identity;
using AuthServer.Application.Services.Account.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using AuthServer.Application.Common.Models.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AuthServer.Application.Common.Models.Settings.Application;
using Microsoft.Extensions.Options;

namespace AuthServer.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ApplicationSettings _applicationSettings;

        public AccountController(
            IAccountService accountService,
            IOptions<ApplicationSettings> applicationSettings)
        {
            _accountService = accountService;
            _applicationSettings = applicationSettings.Value;
        }

        [HttpGet("signin")]
        public IActionResult SignIn([FromQuery] string provider, [FromQuery] string redirectUri)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = $"{_applicationSettings.BaseUrl}/api/account/callback?redirectUri={redirectUri}"
            };

            return Challenge(props, provider);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string redirectUri)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
                return Unauthorized();

            var callbackResult = await _accountService.Callback(result);

            redirectUri += callbackResult.Result?.Token + $"&type={callbackResult.Result?.Type}" + $"&refreshtoken={callbackResult?.Result?.RefreshToken}";
            
            return Redirect(redirectUri);
        }

        [AllowAnonymous]
        [HttpPost(nameof(Register))]
        public async Task<ActionResult<ResultResponse<AuthResponse>>> Register([FromBody] RegistrationRequestDto request)
        {
            var result = await _accountService.RegisterAsync(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost(nameof(Authenticate))]
        public async Task<ActionResult<ResultResponse<AuthResponse>>> Authenticate([FromBody] AuthRequestDto request)
        {
            var result = await _accountService.AuthenticateAsync(request);
            return Ok(result);  
        }

        [AllowAnonymous]
        [HttpPost(nameof(RefreshToken))]
        public async Task<ActionResult<ResultResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _accountService.RefreshToken(request);
            return Ok(result);
        }

        [HttpPost(nameof(DeleteRefreshToken))]
        public async Task<ActionResult<ResultResponse<AuthResponse>>> DeleteRefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _accountService.DeleteRefreshToken(request);
            return Ok(result);
        }

        [Authorize(Roles = "User,Admin,Staff")]
        [HttpPost(nameof(ChangePassword))]
        public async Task<ActionResult<AuthResponse>> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var result = await _accountService.ChangePassword(request);
            return Ok(result);
        }

        [Authorize(Roles = "User,Admin,Staff")]
        [HttpPost(nameof(UpdateEmail))]
        public async Task<ActionResult<BaseResponse>> UpdateEmail([FromBody] UpdateEmailDto request)
        {
            var result = await _accountService.UpdateEmail(request);
            return Ok(result);
        }

        [Authorize(Roles = "User,Admin,Staff")]
        [HttpPost(nameof(UpdateName))]
        public async Task<ActionResult<BaseResponse>> UpdateName([FromBody] UpdateNameDto request)
        {
            var result = await _accountService.UpdateName(request);
            return Ok(result);
        }

        [Authorize(Roles = "User,Admin,Staff")]
        [HttpPost(nameof(UpdatePhone))]
        public async Task<ActionResult<BaseResponse>> UpdatePhone([FromBody] UpdatePhoneDto request)
        {
            var result = await _accountService.UpdatePhone(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost(nameof(SendForgotPasswordEmail))]
        public async Task<ActionResult<BaseResponse>> SendForgotPasswordEmail([FromBody] ForgotPassword request)
        {
            var result = await _accountService.SendForgotPasswordEmail(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost(nameof(VerifyForgotPasswordOTP))]
        public async Task<ActionResult<BaseResponse>> VerifyForgotPasswordOTP([FromBody] VerifyOTPDto request)
        {
            var result = await _accountService.VerifyForgotPasswordOTP(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost(nameof(CreateNewPassword))]
        public async Task<ActionResult<BaseResponse>> CreateNewPassword([FromBody] CreateNewPasswordDto request)
        {
            var result = await _accountService.CreateNewPassword(request);
            return Ok(result);
        }

        [HttpGet(nameof(GetAccountDetails))]
        public async Task<ActionResult<PagedListResponse<List<AccountDetailDto>>>> GetAccountDetails(
           [FromQuery] GetPagedAccountDetailDto getPagedAccountDetailDto)
        {
            var result = await _accountService.GetAccountDetails(getPagedAccountDetailDto);
            return Ok(result);
        }

        [HttpPut(nameof(UpdateAccountDetails))]
        public async Task<ActionResult<BaseResponse>>
            UpdateAccountDetails(AccountDetailDto accountDetailDto)
        {
            var result = await _accountService.UpdateAccountDetails(accountDetailDto);
            return Ok(result);
        }

        [HttpPut(nameof(UpdateAccountStatus))]
        public async Task<ActionResult<BaseResponse>>
            UpdateAccountStatus(UpdateAccountStatusDto accountStatusDto)
        {
            var result = await _accountService.UpdateAccountStatus(accountStatusDto);
            return Ok(result);
        }
    }
}
