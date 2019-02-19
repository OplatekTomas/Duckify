using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Duckify.Services {
    public class EmailSender : IEmailSender {

        public EmailSender(IOptions<AuthMessageSenderOptions> options) {
            Options = options.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message) {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email) {
            var client = new SendGridClient(apiKey);
            var emailBody = new SendGridMessage() {
                From = new EmailAddress("administration@example.com", "Duckify"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            emailBody.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(emailBody);
        }

    }
}
