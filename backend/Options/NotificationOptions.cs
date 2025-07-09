namespace backend.Options
{
    public class NotificationOptions
    {
        public string DefaultTemplate { get; set; } = "Отсутствует";
        public string[] DefaultEmails { get; set; } = System.Array.Empty<string>();
    }
}
