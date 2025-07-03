using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly AppDbContext _context;

    public HelloController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        var row = _context.Hello.Find(1);
        return row == null ? NotFound() : Ok(row.Text);
    }
}
