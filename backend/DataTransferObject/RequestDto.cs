namespace backend.DataTransferObject
{
    public class RequestDto
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreationDate { get; set; }
        public string? ClientName { get; set; }
        public bool IsCritical { get; set; }
    }

}
