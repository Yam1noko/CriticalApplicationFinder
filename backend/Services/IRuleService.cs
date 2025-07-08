namespace backend.Services
{
    using backend.Models.Internal;
    using backend.DataTransferObject;

    public interface IRuleService
    {
        Task<List<RuleDto>> GetAllRules();
        Task AddRule(RuleDto rule);
        Task UpdateRule(int id, RuleDto rule);
        Task DeleteRule(int id);
        Task<List<RuleSubstring>> GetAllSubstrings();
        Task<List<RuleFullName>> GetAllFullNames();
        Task<Request> IsRequestCritical(Request request);
    }
}