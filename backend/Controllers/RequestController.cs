using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using backend.Services;

[ApiController]
[Route("api/requests")]
public class RequestController : ControllerBase
{
    private readonly IRequestRepository _repo;
    private readonly IRequestService _service;

    public RequestController(IRequestRepository repo, IRequestService service)
    {
        _repo = repo;
        _service = service;
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetInRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (from > to)
        {
            return BadRequest("Invalid date range: 'from' must be earlier than 'to'.");
        }
        var list = await _service.GetRequestsInRange(from, to);
        return Ok(list) ;
    }

    [HttpGet("getFirst")]
    public async Task<IActionResult> GetFirst()
    {
        DateTime from = new DateTime(2000, 1, 1);
        DateTime to = new DateTime(3000, 1, 1);
        var list = (await _service.GetRequestsInRange(from, to)).FirstOrDefault();
        return Ok(list);
    }

    [HttpPost("manual-check")]
    public async Task<IActionResult> ManualCheck()
    {
        try
        {
            var result = await _service.ManualCheck();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}



