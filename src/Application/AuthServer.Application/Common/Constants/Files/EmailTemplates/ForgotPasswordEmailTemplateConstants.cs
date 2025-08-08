
namespace AuthServer.Application.Common.Constants.Files.EmailTemplates
{
    internal static class ForgotPasswordEmailTemplateConstants
    {
        public const string EmailTemplateName = "Forgot-Password-Email-Template.html";
        public const string CustomerName = "%{CustomerName}%";
        public const string OTP = "%{OTP}%";
    }
}
