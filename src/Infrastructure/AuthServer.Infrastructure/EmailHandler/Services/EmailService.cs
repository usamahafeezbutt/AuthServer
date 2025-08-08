using Microsoft.Extensions.Options;
using AuthServer.Application.Common.Interfaces.Email;
using AuthServer.Application.Common.Models.Options;
using AuthServer.Application.Common.Models.Settings.Email;
using System.Net;
using System.Net.Mail;
using System.Text;
using AuthServer.Application.Common.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace AuthServer.Infrastructure.EmailHandler.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings smtpSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<SmtpSettings> options,
            ILogger<EmailService> logger)
        {
            smtpSettings = options.Value;
            _logger = logger;
        }

        public async Task<BaseResponse> SendEmail(EmailOptions emailOptions)
        {
            try
            {
                var mail = new MailMessage
                {
                    Subject = emailOptions.Subject,
                    Body = emailOptions.Body,
                    From = new MailAddress(smtpSettings.SenderAddress),
                    IsBodyHtml = smtpSettings.IsBodyHtml,
                };

                PrepareRecipients(emailOptions, mail);
                AddAttachments(emailOptions.Files!, mail);

                var networkCredential = new NetworkCredential(
                    smtpSettings.UserName,
                    smtpSettings.Password);

                var smtpClient = new SmtpClient
                {
                    Host = smtpSettings.Host,
                    Port = smtpSettings.Port,
                    EnableSsl = smtpSettings.EnableSsl,
                    UseDefaultCredentials = smtpSettings.UseDefaultCredentials,
                    Credentials = networkCredential
                };
                mail.BodyEncoding = Encoding.Default;
                await smtpClient.SendMailAsync(mail);
                return new BaseResponse(
                    true,
                    "Message sent successfully, Thanks for reaching us out.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while sending email, {0}", ex);
                return new BaseResponse(false, "Message not sent, please try again.");
            }
        }

        private static void PrepareRecipients(EmailOptions emailOptions, MailMessage mail)
        {
            foreach (var toEmail in emailOptions.ToEmails)
            {
                mail.To.Add(new MailAddress(toEmail));
            }
        }

        public static void AddAttachments(List<IFormFile> formFiles, MailMessage mail)
        {

            if (formFiles is not null)
            {
                foreach (var formFile in formFiles)
                {
                    var attachment = CreateAttachment(formFile);
                    mail.Attachments.Add(attachment);
                }
            }
        }



        private static Attachment CreateAttachment(IFormFile formFile)
        {
            var attachment = new Attachment(formFile.OpenReadStream(), formFile.FileName);

            // Set the content type based on the file type
            attachment.ContentType.MediaType = "application/octet-stream";

            return attachment;
        }
    }
}
