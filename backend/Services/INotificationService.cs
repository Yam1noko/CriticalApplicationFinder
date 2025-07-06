namespace backend.Services
{
    using backend.DataTransferObject;

    public interface INotificationService
    {
        Task<NotificationDTO> GetNotification();

        Task UpdateTemplate(string template);
        Task PostNotification(NotificationDTO notification);
    }

}
