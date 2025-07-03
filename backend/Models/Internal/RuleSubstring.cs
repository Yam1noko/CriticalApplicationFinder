public class RuleSubstring
{
    public int Id { get; set; }
    public string Substring { get; set; } = "";

    public ICollection<Rule> Rules { get; set; } = new List<Rule>();
}
