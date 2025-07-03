using backend.Models.Internal;

public class Rule
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Substring { get; set; } = "";
    public bool UseAnd { get; set; }
    public bool IsActive { get; set; }

    public RuleFullName? RuleFullName { get; set; }
    public RuleSubstring? RuleSubstring { get; set; }
}
