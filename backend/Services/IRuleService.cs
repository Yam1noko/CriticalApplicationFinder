namespace backend.Services
{
    using backend.Models.Internal;

    public interface IRuleService
    {
        Task<List<Rule>> GetAllRules();
        Task AddRule(Rule rule);
        Task UpdateRule(int id, Rule rule);
        Task DeleteRule(int id);
        Task<List<RuleSubstring>> GetAllSubstrings();
        Task<List<RuleFullName>> GetAllFullNames();
        Task<Request> IsRequestCritical(Request request);
    }
}