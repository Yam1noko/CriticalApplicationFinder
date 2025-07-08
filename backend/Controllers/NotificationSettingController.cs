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
        return Ok(list);
    }

    [HttpPost("notificationEmailPost")]
    public async Task<IActionResult> EmailPost([FromQuery] string email)
    {
        var result = await _serv.PostEmail(email);
        if (result)
        {
            return Created();
        }
        else
        {
            return BadRequest();
        }
    }
    [HttpDelete("notificationEmailRemove")]
    public async Task<IActionResult> EmailRemove([FromQuery] string email)
    {
        var result = await _serv.DeleteEmail(email);
        if (result)
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }   

    [HttpPut("notificationTemplateUpdate")]
    public async Task<IActionResult> TemplateUpdate([FromQuery] string template)
    {
        var result = await _serv.UpdateTemplate(template);
        if (result)
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("notificationSendEmail")]
    public async Task<IActionResult> SendEmail()
    {
        var req = new Request
        {
            ClientName = "Вася Пупкин",
            Id = "0001",
            ShortDescr = "Я не могу",
            DescriptionRtf4096 = "Я могу"
        };
        await _serv.SendEmail(req);
        return NoContent();
    }
}
