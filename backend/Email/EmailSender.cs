using backend.Models.Internal;
using Scriban;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace backend.Email
{
    public class EmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _from;
        private readonly string _username;
        private readonly string _password;

        public EmailSender(string host, int port, string from, string username, string password)
        {
            _host = host;
            _port = port;
            _from = from;
            _username = username;
            _password = password;
        }

        public async Task SendAsync(Request request, string templateText, string recipient, string? fromOverride = null)
        {
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

            var from = fromOverride ?? _from;
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(recipient));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls); 
            await client.AuthenticateAsync(_username, _password);
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

