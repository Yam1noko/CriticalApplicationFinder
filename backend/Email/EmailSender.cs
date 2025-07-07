using backend.Models.Internal;
using System.Net;
using System.Net.Mail;

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

        public async Task SendAsync(string subject, string body, string recipient, string? fromOverride = null)
        {
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
