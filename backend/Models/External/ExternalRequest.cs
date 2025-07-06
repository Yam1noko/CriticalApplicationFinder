using Microsoft.EntityFrameworkCore;

namespace backend.Models.External;


[Index(nameof(CreationDate), IsDescending = new[] { true })]
public class ExternalRequest
{
    public string ServiceId { get; set; } = "";
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime CreationDate { get; set; }
    public string ClientName { get; set; } = "";
    public string ShortDescr { get; set; } = "";
    public string DescriptionRtf4096 { get; set; } = "";
}
