using backend.Models.Internal;
using backend.Options;
using Scriban;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

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
                description = request.DescriptionRtf4096,
                link = GenerateLink(request.Id)
            });

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
        }

        private string GenerateLink(string id)
        {
            var cleanId = (id ?? string.Empty).Replace(" ", string.Empty);
            return $"https://sd.fesco.com/sd/operator/#uuid:serviceCall${cleanId}";
        }
    }

}

