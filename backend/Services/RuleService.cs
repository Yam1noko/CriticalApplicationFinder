namespace backend.Services
{
    using backend.DataTransferObject;
    using backend.Models.Internal;
    using backend.Repositories;

    public class RuleService : IRuleService
    {
        private readonly IRuleRepository _repo;
        private readonly RuleEngine _ruleEngine;

        public RuleService(IRuleRepository repo)
        {
            _repo = repo;
            _ruleEngine = new RuleEngine(repo);
        }

        public async Task<List<RuleDto>> GetAllRules()
        {
            var rules = await _repo.GetAll();

            return rules.Select(r => new RuleDto
            {
                Id = r.Id,
                Name = r.Name,
                UseAnd = r.UseAnd,
                IsActive = r.IsActive,
                RuleFullNames = r.RuleFullNames?.Select(f => f.FullName).ToList() ?? new(),
                RuleSubstrings = r.RuleSubstrings?.Select(s => s.Substring).ToList() ?? new()
            }).ToList();
        }

        public async Task AddRule(RuleDto ruleDto)
        {
            var ruleEntity = new Rule
            {
                Name = ruleDto.Name,
                UseAnd = ruleDto.UseAnd,
                IsActive = ruleDto.IsActive,
            };


            await _repo.Add(ruleEntity);


            var fullNames = ruleDto.RuleFullNames.Select(name => new RuleFullName
            {
                RuleId = ruleEntity.Id,
                FullName = name
            }).ToList();

            var substrings = ruleDto.RuleSubstrings.Select(sub => new RuleSubstring
            {
                RuleId = ruleEntity.Id,
                Substring = sub
            }).ToList();


            var efRepo = _repo as EFRuleRepository;
            if (efRepo == null)
                throw new InvalidOperationException("RuleRepository must be EFRuleRepository to allow direct access to DbContext.");

            var context = efRepo.GetDbContext(); 

            context.RuleFullNames.AddRange(fullNames);
            context.RuleSubstrings.AddRange(substrings);

            await context.SaveChangesAsync();
        }

        public async Task UpdateRule(int id, RuleDto ruleDto)
        {
            if (id != ruleDto.Id)
                throw new ArgumentException("ID in route does not match ID in request body");

            var existingRule = await _repo.GetById(id);
            if (existingRule == null)
                throw new KeyNotFoundException($"Rule with id {id} not found");

  
            existingRule.Name = ruleDto.Name;
            existingRule.UseAnd = ruleDto.UseAnd;
            existingRule.IsActive = ruleDto.IsActive;

            
            await _repo.RemoveSubstringsByRuleId(id);
            await _repo.RemoveFullNamesByRuleId(id);

            var fullNames = ruleDto.RuleFullNames.Select(name => new RuleFullName
            {
                RuleId = existingRule.Id,
                FullName = name
            }).ToList();

            var substrings = ruleDto.RuleSubstrings.Select(sub => new RuleSubstring
            {
                RuleId = existingRule.Id,
                Substring = sub
            }).ToList();


            var efRepo = _repo as EFRuleRepository;
            if (efRepo == null)
                throw new InvalidOperationException("RuleRepository must be EFRuleRepository to allow direct access to DbContext.");

            var context = efRepo.GetDbContext();

            context.RuleFullNames.AddRange(fullNames);
            context.RuleSubstrings.AddRange(substrings);

            await context.SaveChangesAsync();
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

        public async Task<Request> IsRequestCritical(Request request)
        {
            
            var allRules = await _repo.GetAll();
            var activeRules = allRules.Where(r => r.IsActive).ToList();


            var engineRequest = request;


            foreach (var rule in activeRules)
            {
                bool isMatch = await _ruleEngine.IsRequestCorrespondToRule(engineRequest, rule);
                if (isMatch)
                {
                    request.isCritical = true;
                    return request;
                }
            }


            return request;
        }
    }
}