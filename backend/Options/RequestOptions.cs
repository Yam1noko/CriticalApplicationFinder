namespace backend.Options
{
    public class RequestOptions
    {
        public DateTime FirstRequestFrom { get; set; } = new DateTime(2000, 1, 1);
        public DateTime FirstRequestTo { get; set; } = new DateTime(3000, 1, 1);
    }
}
