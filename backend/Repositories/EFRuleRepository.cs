using backend.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class EFRuleRepository : IRuleRepository
{
    private readonly InternalDbContext _context;

    public EFRuleRepository(InternalDbContext context)
    {
        _context = context;
    }

    public async Task<Rule?> GetById(int id)
    {
        return await _context.Rules.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<List<Rule>> GetAll()
    {
        return await _context.Rules.ToListAsync();
    }
    public async Task Add(Rule rule)
    {
        _context.Rules.Add(rule);
        await _context.SaveChangesAsync();
    }
    public async Task Remove(Rule rule)
    {
        _context.Rules.Remove(rule);
        await _context.SaveChangesAsync();
    }
    public async Task Update(Rule rule)
    {
        _context.Rules.Update(rule);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RuleSubstring>> GetAllSubstrings()
    {
        return await _context.RuleSubstrings.ToListAsync();
    }

    public async Task<List<RuleFullName>> GetAllFullNames()
    {
        return await _context.RuleFullNames.ToListAsync();  
    }
}