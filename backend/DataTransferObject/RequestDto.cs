namespace backend.DataTransferObject
{
    public class RequestDto
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string? ClientName { get; set; }
        public string? ShortDescr { get; set; }
        public string? DescriptionRtf4096 {  get; set; }
        public bool IsCritical { get; set; }
    }

}
