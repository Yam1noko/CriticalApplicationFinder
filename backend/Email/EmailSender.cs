using backend.Models.Internal;
using System.Net;
using System.Net.Mail;
using Scriban;

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
                description = request.DescriptionRtf4096
            });

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
        }

    }
}