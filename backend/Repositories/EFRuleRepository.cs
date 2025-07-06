using System.Text.Json;
using backend.Data;
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
        return await _context.Rules
            .Include(r => r.RuleFullNames)
            .Include(r => r.RuleSubstrings)
            .AsNoTracking()
            .ToListAsync();
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
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var existingRule = await _context.Rules
                .Include(r => r.RuleSubstrings)
                .Include(r => r.RuleFullNames)
                .FirstAsync(r => r.Id == rule.Id);

            existingRule.Name = rule.Name;
            existingRule.UseAnd = rule.UseAnd;
            existingRule.IsActive = rule.IsActive;

            if (rule.RuleSubstrings != null)
            {
                _context.RuleSubstrings.RemoveRange(existingRule.RuleSubstrings);

                foreach (var sub in rule.RuleSubstrings)
                {
                    _context.RuleSubstrings.Add(new RuleSubstring
                    {
                        Id = sub.Id,
                        RuleId = rule.Id,
                        Substring = sub.Substring
                    });
                }
            }

            if (rule.RuleFullNames != null)
            {
                _context.RuleFullNames.RemoveRange(existingRule.RuleFullNames);

                foreach (var name in rule.RuleFullNames)
                {
                    _context.RuleFullNames.Add(new RuleFullName
                    {
                        Id = name.Id,
                        RuleId = rule.Id,
                        Surname = name.Surname,
                        Name = name.Name,
                        Patronymic = name.Patronymic
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
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