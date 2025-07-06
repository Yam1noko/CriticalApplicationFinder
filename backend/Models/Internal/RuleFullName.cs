using System.ComponentModel.DataAnnotations.Schema;

[Table("RuleFullNames")]
public class RuleFullName
{
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey(nameof(Rule))]
    [Column("rule_id")]
    public int RuleId { get; set; }

    [Column("surname")]
    public string Surname { get; set; } = "";

    [Column("name")]
    public string Name { get; set; } = "";

    [Column("patronymic")]
    public string Patronymic { get; set; } = "";
}
