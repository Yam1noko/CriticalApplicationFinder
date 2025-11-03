using backend.Models.Internal;
<<<<<<< Updated upstream
using System.Net;
using System.Net.Mail;
using Scriban;
=======
using backend.Options;
using Scriban;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
>>>>>>> Stashed changes

namespace backend.Email
{
    public class EmailSender
    {
        private readonly IOptionsMonitor<EmailOptions> _settings;
        public EmailSender(IOptionsMonitor<EmailOptions> settings)
        {
            _settings = settings;
        }

        public async Task SendAsync(Request request, string templateText, string recipient, string? fromOverride = null)
        {
            var emailSettings = _settings.CurrentValue;
            var subject = $@"Критическая заявка: {request.ShortDescr}";
            var template = Template.Parse(templateText);
            var body = template.Render(new {
                seviceId = request.ServiceId,
                id = request.Id, 
                title = request.Title,
                creationDate = request.CreationDate,
                clientName = request.ClientName,
                shortDescr = request.ShortDescr,
                description = request.DescriptionRtf4096
            });

<<<<<<< Updated upstream
            var from = fromOverride ?? _from;
            var message = new MailMessage(from, recipient, subject, body)
            {
                IsBodyHtml = true
            };

            using var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
=======
            var from = fromOverride ?? emailSettings.From;
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(recipient));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailSettings.Username, emailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
>>>>>>> Stashed changes
        }

    }

}

