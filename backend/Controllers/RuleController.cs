using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class RuleController : ControllerBase
{
    private readonly IRuleRepository _repo;

    public RuleController(IRuleRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetRules() // done.
    {
        var rules = await _repo.GetAll();
        return Ok(rules);
    }

    [HttpPost("rules")]
    public async Task<IActionResult> AddRule([FromBody] Rule rule) // done.
    {
        await _repo.Add(rule);
        return NoContent();
    }

    [HttpPut("rules{id}")]
    public async Task<IActionResult> UpdateRule([FromRoute] string id, [FromBody] Rule rule) // done.
    {
        await _repo.Update(rule);
        return NoContent();
    }

    [HttpDelete("rules{id}")]
    public async Task<IActionResult> DeleteRule([FromRoute] string id)
    {
        var rule = await _repo.GetById(Int32.Parse(id));
        if (rule == null)
        {
            return NotFound();
        }
        await _repo.Remove(rule);
        return NoContent();
    }

    [HttpGet("FullNames")]
    public async Task<IActionResult> GetFullNames()
    {
        var fullNames = await _repo.GetAllFullNames();
        return Ok(fullNames);
    }

    [HttpGet("substrings")]
    public async Task<IActionResult> GetSubstrings()
    {
        var substrings = await _repo.GetAllSubstrings();
        return Ok(substrings);
    }
}