namespace backend.Repositories;

public interface IRuleRepository
{
    Task<List<Rule>> GetAll();
    Task Add(Rule rule);
    Task Remove(Rule rule);
    Task Update(Rule rule);
    Task<Rule?> GetById(int id);
    Task<List<RuleSubstring>> GetAllSubstrings();
    Task<List<RuleFullName>> GetAllFullNames();
}