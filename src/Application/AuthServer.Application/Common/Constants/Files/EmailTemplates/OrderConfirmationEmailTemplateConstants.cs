
namespace AuthServer.Application.Common.Constants.Files.EmailTemplates
{
    public class OrderConfirmationEmailTemplateConstants
    {
        public const string EmailTemplateName = "Ainssist-Order-Confirmation-Email-template.html";

        public const string CustomerName = "%{CustomerName}%";

        public const string OrderNumber = "%{OrderNumber}%";
        public const string OrderDate = "%{OrderDate}%";

        public const string ProductImage = "%{ProductImage}%";
        public const string ProductName = "%{ProductName}%";
        public const string ProductColor = "%{ProductColor}%";
        public const string ProductSize = "%{ProductSize}%";
        public const string ProductQuantity = "%{ProductQuantity}%";
        public const string ProductPrice = "%{ProductPrice}%";

        public const string Subtotal = "%{Subtotal}%";
        public const string Discount = "%{Discount}%";
        public const string Shipping = "%{Shipping}%";
        public const string Total = "%{Total}%";

        public const string Address = "%{Address}%";
        public const string AdditionalAddress = "%{AdditionalAddress}%";
        public const string City = "%{City}%";
        public const string State = "%{State}%";
        public const string Country = "%{Country}%";

        public const string PaymentMethod = "%{PaymentMethod}%";
        public const string ShippingMethod = "%{ShippingMethod}%";
    }
}
