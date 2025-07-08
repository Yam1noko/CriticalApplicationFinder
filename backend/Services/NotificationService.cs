namespace backend.Services
{
    using backend.DataTransferObject;
    using backend.Models.Internal;
    using backend.Repositories;
    using backend.Email;
    using Scriban;

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _Repo;
        private readonly EmailSender _Sender;


        private NotificationDTO _notificationDTO;

        public NotificationService(INotificationRepository repo, EmailSender sender)
        {
            _Repo = repo;
            _Sender = sender;
            _notificationDTO = new NotificationDTO
            {
                Template = "Отсутствует",
                Emails = Array.Empty<string>()
            };
        }

        public async Task<NotificationDTO> GetNotification()
        {
            if ((_notificationDTO.Template == "Отсутствует") || (_notificationDTO.Emails.Length == 0))
            {
                await UpdateNotification();
                return _notificationDTO;
            }
            else
            {
                return _notificationDTO;
            }
        }

        private async Task UpdateNotification()
        {
            var template = await _Repo.GetTemplate();
            List<NotificationEmail> mailsReq = await _Repo.GetAllEmails();

            List<string> mails = new List<string> { };

            foreach (var item in mailsReq)
            {
                mails.Add(item.Address);
            }

            _notificationDTO.Emails = mails.ToArray();
            _notificationDTO.Template = template.Template;
        }

        public async Task<bool> UpdateTemplate(string template) {
            var templat = Template.Parse(template);
            if (templat.HasErrors) 
            { 
                return false; 
            }
            var templ = new NotificationTemplate();
            templ.Template = template;
            templ.Id = 1;
            if (await _Repo.ExistTemplate())
            {
                _notificationDTO.Template = template;
                _Repo.UpdateTemplate(templ);
                return true;
            }
            else
            {
                _notificationDTO.Template = template;
                _Repo.AddTemplate(templ);
                return true;
            }
        }
        
        public async Task<bool> PostEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                return false;
            }
            else if (await _Repo.ExistEmail(email))
            {
                return false;
            }
            else
            {
                var mail = new NotificationEmail
                {
                    Id = await _Repo.FindId("max") + 1,
                    Address = email
                };

                List<string> mmails = new List<string> { };

                foreach (var item in _notificationDTO.Emails)
                {
                    mmails.Add(item);
                }

                mmails.Add(email);

                _notificationDTO.Emails = mmails.ToArray();


                await _Repo.AddEmail(mail);
                return true;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                return false;
            }
            else if (!(await _Repo.ExistEmail(email)))
            {
                return false;
            }
            else
            {
                var mail = await _Repo.GetEmailByAddress(email);

                List<string> mmails = new List<string> { };

                foreach (var item in _notificationDTO.Emails)
                {
                    if (item != email) 
                    { mmails.Add(item); }
                }
                
                _notificationDTO.Emails = mmails.ToArray();
                await _Repo.RemoveEmail(mail);
                return true;
            }
        }

        public async Task SendEmail(Request request)
        {
            if (_notificationDTO.Template == "Отсутствует")
            {
                await UpdateNotification();
            }
            foreach (var item in _notificationDTO.Emails)
            {
                await _Sender.SendAsync(request, _notificationDTO.Template, item);
            }
        }

    }
}
