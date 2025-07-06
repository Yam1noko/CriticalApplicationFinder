namespace backend.Services
{
    using backend.DataTransferObject;
    using backend.Models;
    using backend.Models.External;
    using backend.Models.Internal;
    using backend.Repositories;
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _Repo;

        private NotificationDTO _notificationDTO;

        public NotificationService(INotificationRepository repo)
        {
            _Repo = repo;
            _notificationDTO = new NotificationDTO();
            _notificationDTO.Template = "Отсутствует";
        }

        public async Task<NotificationDTO> GetNotification()
        {
            if (_notificationDTO.Template == "Отсутствует")
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
                return _notificationDTO;
            }
            else
            {
                return _notificationDTO;
            }
        }

        public async Task UpdateTemplate(string template) {
            var templ = new NotificationTemplate();
            templ.Template = template;
            templ.Id = 1;
            if (await _Repo.ExistTemplate())
            {
                _Repo.UpdateTemplate(templ);
            }
            else
            {
                _Repo.AddTemplate(templ);
            }
        }

        public async Task PostNotification(NotificationDTO notification) { }
    }
}
