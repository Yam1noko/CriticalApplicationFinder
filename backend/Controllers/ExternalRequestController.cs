using backend.Models.External;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/external-requests")]
    public class ExternalRequestController : ControllerBase
    {
        private readonly IExternalRequestRepository _repo;

        public ExternalRequestController(IExternalRequestRepository repo)
        {
            _repo = repo;
        }

        // GET: api/external-requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalRequest>>> GetAll()
        {
            var requests = await _repo.GetAllAsync();
            return Ok(requests);
        }

        // GET: api/external-requests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExternalRequest>> GetById(string id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null)
                return NotFound();
            return Ok(request);
        }

        // GET: api/external-requests/range?from=2024-01-01&to=2024-12-31
        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<ExternalRequest>>> GetInRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var requests = await _repo.GetInRangeAsync(from, to);
            return Ok(requests);
        }
    }
}
