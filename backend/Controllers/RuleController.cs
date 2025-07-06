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
    public async Task<IActionResult> GetRules()
    {
        try {
            return Ok(await _repo.GetAll());
        }
        catch (Exception ex) {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("rules")]
    public async Task<IActionResult> AddRule([FromBody] Rule rule)
    {
        await _repo.Add(rule);
        return NoContent();
    }

    [HttpPut("rules/{id}")]
    public async Task<IActionResult> UpdateRule([FromRoute] int id, [FromBody] Rule rule)
    {
        if (id != rule.Id)
        {
            return BadRequest("ID in route does not match ID in request body");
        }

        try
        {
            await _repo.Update(rule);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("rules/{id}")]
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