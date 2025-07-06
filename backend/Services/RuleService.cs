namespace backend.Services
{
    using backend.DataTransferObject;
    using backend.Repositories;
    using backend.Models.Internal;

    public class RuleService : IRuleService
    {
        private readonly IRuleRepository _repo;
        private readonly RuleEngine _ruleEngine;

        public RuleService(IRuleRepository repo)
        {
            _repo = repo;
            _ruleEngine = new RuleEngine(repo);
        }

        public async Task<List<Rule>> GetAllRules()
        {
            return await _repo.GetAll();
        }

        public async Task AddRule(Rule rule)
        {
            await _repo.Add(rule);
        }

        public async Task UpdateRule(int id, Rule rule)
        {
            if (id != rule.Id)
            {
                throw new ArgumentException("ID in route does not match ID in request body");
            }

            await _repo.Update(rule);
        }

        public async Task DeleteRule(int id)
        {
            var rule = await _repo.GetById(id);
            if (rule == null)
            {
                throw new KeyNotFoundException($"Rule with id {id} not found");
            }

            await _repo.Remove(rule);
        }

        public async Task<List<RuleSubstring>> GetAllSubstrings()
        {
            return await _repo.GetAllSubstrings();
        }

        public async Task<List<RuleFullName>> GetAllFullNames()
        {
            return await _repo.GetAllFullNames();
        }

        public async Task<RequestDto> IsRequestCritical(RequestDto request)
        {
            // Получаем все активные правила
            var allRules = await _repo.GetAll();
            var activeRules = allRules.Where(r => r.IsActive).ToList();

            // Создаем Request для RuleEngine
            var engineRequest = new Request
            {
                ClientName = request.ClientName ?? "",
                DescriptionRtf4096 = request.DescriptionRtf4096 ?? ""
            };

            // Проверяем соответствие каждому активному правилу
            foreach (var rule in activeRules)
            {
                bool isMatch = await _ruleEngine.IsRequestCorrespondToRule(engineRequest, rule);
                if (isMatch)
                {
                    return new RequestDto
                    {
                        Id = request.Id,
                        CreationDate = request.CreationDate,
                        ClientName = request.ClientName,
                        ShortDescr = request.ShortDescr,
                        DescriptionRtf4096 = request.DescriptionRtf4096,
                        IsCritical = true
                    };
                }
            }

            // Если не соответствует ни одному правилу
            return new RequestDto
            {
                Id = request.Id,
                CreationDate = request.CreationDate,
                ClientName = request.ClientName,
                ShortDescr = request.ShortDescr,
                DescriptionRtf4096 = request.DescriptionRtf4096,
                IsCritical = false
            };
        }
    }
}