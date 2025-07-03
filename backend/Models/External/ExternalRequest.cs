namespace backend.Models.External;

public class ExternalRequest
{
    public string ServiceId { get; set; } = "";
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string CreationDate { get; set; } = "";
    public string ClientName { get; set; } = "";
    public string ShortDescr { get; set; } = "";
    public string DescriptionRtf4096 { get; set; } = "";
}
