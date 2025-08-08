
namespace AuthServer.Application.Common.Constants.Files.EmailTemplates
{
    public static class CustomerSupportRequestEmailTemplateConstants
    {
        public const string EmailTemplateName = "Customer-Support-Request.html";
        public const string CustomerName = "%{CustomerName}%";
        public const string SupportRequestNumber = "%{SupportRequestNumber}%";
        public const string OrderNumber = "%{OrderNumber}%";
    }
}
