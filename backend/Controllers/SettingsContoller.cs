using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;


[Route("api/Settings")]
public class SettingsController : ControllerBase
{
    private readonly string _configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    [HttpGet("settingsGetSettings")]
    public IActionResult GetSettings()
    {
        if (!System.IO.File.Exists(_configPath))
            return NotFound("Файл appsettings.json не найден.");

        var json = System.IO.File.ReadAllText(_configPath);
        var jsonObject = JsonNode.Parse(json);
        return Ok(jsonObject);
    }

    [HttpPost("settingsUpdateSettings")]
    public IActionResult UpdateSettings([FromBody] JsonObject newSettings)
    {
        try
        {
            if (newSettings is null)
            {
                return BadRequest("Пустое тело запроса");
            }

            var json = System.IO.File.ReadAllText(_configPath);
            var currentSettings = JsonNode.Parse(json)?.AsObject();

            if (currentSettings is null)
            {
                return StatusCode(500, "Ошибка чтения текущего файла настроек");
            }

            var changed = MergeJson(currentSettings, newSettings);

            if (changed)
            {
                System.IO.File.WriteAllText(_configPath, currentSettings.ToJsonString());
            }

            return Ok("Настройки успешно обновлены.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Произошла ошибка: {ex.Message}");
        }
    }

    private bool MergeJson(JsonObject target, JsonObject source)
    {
        bool changed = false;

        foreach (var kv in source)
        {
            if (kv.Value is JsonObject srcObj)
            {
                if (target[kv.Key] is not JsonObject tgtObj)
                {
                    target[kv.Key] = JsonNode.Parse(srcObj.ToJsonString())!;
                    changed = true;
                }
                else if (MergeJson(tgtObj, srcObj))
                {
                    changed = true;
                }
            }
            else if (kv.Value is JsonArray srcArray)
            {
                target[kv.Key] = JsonNode.Parse(srcArray.ToJsonString())!;
                changed = true;
            }
            else
            {
                string? oldValue = target[kv.Key]?.ToJsonString();
                string? newValue = kv.Value?.ToJsonString();

                if (oldValue != newValue)
                {
                    target[kv.Key] = kv.Value is null
                        ? null
                        : JsonNode.Parse(kv.Value.ToJsonString())!;
                    changed = true;
                }
            }
        }

        return changed;
    }


}