using FeedHiveAuth.Models.Common;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FeedHiveAuth.Data.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        public EmailSender(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtp = new SmtpClient(_settings.Server, _settings.Port)
            {
                Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password),
                EnableSsl = true
            };
            var message = new MailMessage
            {
                From = new MailAddress(_settings.Username),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(email);
            return smtp.SendMailAsync(message);

        }
    }
}
