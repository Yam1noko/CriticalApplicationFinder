using backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class RuleController : ControllerBase
{
    private readonly IRuleService _service;

    public RuleController(IRuleService service)
    {
        _service = service;
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetRules()
    {
        try 
        {
            var rules = await _service.GetAllRules();
            return Ok(rules);
        }
        catch (Exception ex) 
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("rules")]
    public async Task<IActionResult> AddRule([FromBody] Rule rule)
    {
        try
        {
            await _service.AddRule(rule);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("rules/{id}")]
    public async Task<IActionResult> UpdateRule([FromRoute] int id, [FromBody] Rule rule)
    {
        try
        {
            await _service.UpdateRule(id, rule);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("rules/{id}")]
    public async Task<IActionResult> DeleteRule([FromRoute] int id)
    {
        try
        {
            await _service.DeleteRule(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("FullNames")]
    public async Task<IActionResult> GetFullNames()
    {
        try
        {
            var fullNames = await _service.GetAllFullNames();
            return Ok(fullNames);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("substrings")]
    public async Task<IActionResult> GetSubstrings()
    {
        try
        {
            var substrings = await _service.GetAllSubstrings();
            return Ok(substrings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}