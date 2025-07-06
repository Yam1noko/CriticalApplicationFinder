using System.ComponentModel.DataAnnotations.Schema;

[Table("RuleSubstrings")]
public class RuleSubstring
{
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey(nameof(Rule))]
    [Column("rule_id")]
    public int RuleId { get; set; }

    [Column("substring")]
    public string Substring { get; set; } = "";
}
