using System.ComponentModel.DataAnnotations.Schema;
using backend.Repositories;

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

    public async Task PopulateFromDatabase(IRuleRepository repository)
    {
        var fullNames = await repository.GetFullNamesByRuleId(this.Id);
        this.RuleFullNames = fullNames?.ToList() ?? new List<RuleFullName>();

        var substrings = await repository.GetSubstringsByRuleId(this.Id);
        this.RuleSubstrings = substrings?.ToList() ?? new List<RuleSubstring>();
    }
}
