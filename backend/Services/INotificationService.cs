namespace backend.Services
{
    using backend.DataTransferObject;

    public interface INotificationService
    {
        Task<NotificationDTO> GetNotification();

        Task<bool> UpdateTemplate(string template);

        Task<bool> PostEmail (string mail);

        Task<bool> DeleteEmail(string mail);
    }

}
