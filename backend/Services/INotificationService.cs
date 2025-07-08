namespace backend.Services
{
    using backend.DataTransferObject;
    using backend.Models.Internal;

    public interface INotificationService
    {
        Task<NotificationDTO> GetNotification();

        Task<bool> UpdateTemplate(string template);

        Task<bool> PostEmail (string mail);

        Task<bool> DeleteEmail(string mail);

        Task SendEmail(Request request);
    }

}
