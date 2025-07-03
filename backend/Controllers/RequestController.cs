using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/requests")]
public class RequestController : ControllerBase
{
    private readonly IRequestRepository _repo;

    public RequestController(IRequestRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetInRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var list = await _repo.GetByDateRange(from, to);
        return Ok(list);
    }
}
