using backend.Models.Internal;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

[Route("api/notification")]
public class NotificationSettingController : ControllerBase
{
    private readonly INotificationRepository _repo;

    public NotificationSettingController(INotificationRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("notificationEmailGet")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _repo.GetAllEmails();
        return Ok(list);
    }
    [HttpGet("notificationEmailPost")]
    public async Task<IActionResult> EmailPost([FromQuery] string email)
    {
        if (await _repo.ExistEmail(email)) 
        {
            return BadRequest(); 
        }
        else
        {
            var mail = new NotificationEmail
            {
                Id = await _repo.FindId("max")+1,
                Address = email
            };
            await _repo.AddEmail(mail);
            return Created();
        }
    }
    [HttpGet("notificationEmailRemove")]
    public async Task<IActionResult> EmailRemove([FromQuery] string email)
    {
        if (await _repo.ExistEmail(email))
        {
            var mail = new NotificationEmail
            {
                Id = await _repo.FindId(email),
                Address = email
            };
            await _repo.RemoveEmail(mail);
            return Ok(email);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpGet("notificationTemplateGet")]
    public async Task<IActionResult> getTemplate()
    {
        var list = await _repo.GetTemplate();
        return Ok(list);
    }

    [HttpGet("notificationTemplatePost")]
    public async Task<IActionResult> TemplatePost([FromQuery] string template)
    {

        var templ = new NotificationTemplate
        {
            Id = 1,
            Template = template
        };
        await _repo.AddTemplate(templ);
        return Created();
    }

    [HttpGet("notificationTemplatePut")]
    public async Task<IActionResult> TemplatePut([FromQuery] string template)
    {
        var templ = new NotificationTemplate
        {
            Id = 1,
            Template = template
        };
        await _repo.UpdateTemplate(templ);
        return Created();
    }
}
