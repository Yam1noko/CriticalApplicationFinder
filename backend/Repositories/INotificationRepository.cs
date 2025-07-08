using backend.Models.Internal;

namespace backend.Repositories;

public interface INotificationRepository
{
    Task<List<NotificationEmail>> GetAllEmails();
    Task AddEmail(NotificationEmail email);
    Task RemoveEmail(NotificationEmail email);
    Task<bool> ExistEmail(string email);

    Task<int> FindId(string email);

    Task<NotificationTemplate?> GetTemplate();
    Task AddTemplate (NotificationTemplate template);
    Task UpdateTemplate(NotificationTemplate template);
    Task<bool> ExistTemplate();

    Task<NotificationEmail> GetEmailByAddress(string email);
}
