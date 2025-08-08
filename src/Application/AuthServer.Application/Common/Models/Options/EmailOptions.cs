using Microsoft.AspNetCore.Http;

namespace AuthServer.Application.Common.Models.Options
{
    public class EmailOptions
    {
        public List<string> ToEmails { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public List<string>? EmailAttachments { get; set; } = [];
        public List<IFormFile>? Files { get; set; }
    }
}
