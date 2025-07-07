using System.Text.RegularExpressions;
using backend.Models.Internal;
using backend.Repositories;

public class RuleEngine
{
    public RuleEngine(IRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    private readonly IRuleRepository _ruleRepository;

    private string RemoveAllNonAlphanumeric(string s)
    {
        return Regex.Replace(s, "[^\\p{L}\\p{Nd}]", "");
    }

    private string Normalize(string s)
    {
        return RemoveAllNonAlphanumeric(s);
    }

    public async Task<bool> IsRequestCorrespondToRule(Request request, Rule rule)
    {
        await rule.PopulateFromDatabase(_ruleRepository);

        string clientName = request.ClientName ?? "";
        string description = request.DescriptionRtf4096 ?? "";
        
        string normalizedClientName = Normalize(clientName).ToLower();
        string normalizedDescription = Normalize(description).ToLower();

        bool nameMatches = true;
        if (rule.RuleFullNames != null && rule.RuleFullNames.Count > 0)
        {
            nameMatches = false;
            for (int i = 0; i < rule.RuleFullNames.Count; i++)
            {
                RuleFullName fullName = rule.RuleFullNames[i];
                string surname = fullName.Surname != null ? fullName.Surname : "";
                string name = fullName.Name != null ? fullName.Name : "";
                string patronymic = fullName.Patronymic != null ? fullName.Patronymic : "";
                
                string composedName = surname + name + patronymic;
                string normalizedRuleName = Normalize(composedName).ToLower();

                if (normalizedRuleName.Length > 0 && normalizedClientName.Contains(normalizedRuleName))
                {
                    nameMatches = true;
                    break;
                }
            }
        }

        bool textMatches = true;
        if (rule.RuleSubstrings != null && rule.RuleSubstrings.Count > 0)
        {
            textMatches = false;
            for (int i = 0; i < rule.RuleSubstrings.Count; i++)
            {
                RuleSubstring substring = rule.RuleSubstrings[i];
                string sub = substring.Substring ?? "";
                
                string normalizedSub = Normalize(sub).ToLower();

                if (normalizedSub.Length > 0 && normalizedDescription.Contains(normalizedSub))
                {
                    textMatches = true;
                    break;
                }
            }
        }

        if (rule.UseAnd)
        {
            return nameMatches && textMatches;
        }
        else
        {
            return nameMatches || textMatches;
        }
    }
}