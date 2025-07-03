public class RuleFullName
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";

    public ICollection<Rule> Rules { get; set; } = new List<Rule>();
}
