using backend.DataTransferObject;
using backend.Models.Internal;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/notification")]
public class NotificationSettingController : ControllerBase
{
    private readonly INotificationRepository _repo;
    private readonly INotificationService _serv;

    public NotificationSettingController(INotificationRepository repo, INotificationService serv)
    {
        _repo = repo;
        _serv = serv;
    }

    [HttpGet("notificationNotificationGet")]
    public async Task<IActionResult> GetNotification()
    {
        NotificationDTO list = await _serv.GetNotification();
        var str = "<mails>";
        foreach (var item in list.Emails)
        {
            str += item + ", ";
        }
        str += "</email>\n";
        str += list.Template;
        return Ok(str);
    }

    [HttpGet("notificationEmailGet")]
    public async Task<IActionResult> getAllEmais()
    {
        var list = await _repo.GetAllEmails();
        return Ok(list);
    }

    [HttpPost("notificationEmailPost")]
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
                Id = await _repo.FindId("max") + 1,
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

    [HttpGet("notificationTemplateUpdate")]
    public async Task<IActionResult> TemplateUpdate([FromQuery] string template)
    {
        _serv.UpdateTemplate(template);
        return Created();
    }
}
