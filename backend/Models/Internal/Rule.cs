using System.ComponentModel.DataAnnotations.Schema;

[Table("Rules")]
public class Rule
{
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = "";
    [Column("use_and")]
    public bool UseAnd { get; set; }
    [Column("is_active")]
    public bool IsActive { get; set; }

    public List<RuleFullName>? RuleFullNames { get; set; } = new List<RuleFullName>();
    public List<RuleSubstring>? RuleSubstrings { get; set; } = new List<RuleSubstring>();
}
