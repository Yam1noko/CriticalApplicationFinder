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
            .OrderByDescending(r => r.Id)
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
    public InternalDbContext GetDbContext()
    {
        return _context;
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
                        FullName = name.FullName
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
    
    public async Task<List<RuleFullName>> GetFullNamesByRuleId(int ruleId)
    {
        return await _context.RuleFullNames
            .Where(fn => fn.RuleId == ruleId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<RuleSubstring>> GetSubstringsByRuleId(int ruleId)
    {
        return await _context.RuleSubstrings
            .Where(ss => ss.RuleId == ruleId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RemoveSubstringsByRuleId(int ruleId)
    {
        var existing = await _context.RuleSubstrings
            .Where(x => x.RuleId == ruleId)
            .ToListAsync();
        _context.RuleSubstrings.RemoveRange(existing);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFullNamesByRuleId(int ruleId)
    {
        var existing = await _context.RuleFullNames
            .Where(x => x.RuleId == ruleId)
            .ToListAsync();
        _context.RuleFullNames.RemoveRange(existing);
        await _context.SaveChangesAsync();
    }

}