using backend.Options;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/requests")]
public class RequestController : ControllerBase
{
    private readonly IRequestRepository _repo;
    private readonly IRequestService _service;
    private readonly RequestOptions _options;
    public RequestController(IRequestRepository repo, IRequestService service, IOptions<RequestOptions> options)
    {
        _repo = repo;
        _service = service;
        _options = options.Value;
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
        var from = _options.FirstRequestFrom;
        var to = _options.FirstRequestTo;
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



