using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RuleFullNames")]
public class RuleFullName
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Rule))]
    [Column("rule_id")]
    public int RuleId { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = "";
}
