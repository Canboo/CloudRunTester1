using Microsoft.AspNetCore.Mvc;
using Google.Cloud.SecretManager.V1;


namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController()
    {
        _client = SecretManagerServiceClient.Create();
    }

    private readonly SecretManagerServiceClient _client;
    private readonly string _projectId = "tester-gcp-425814";

    [HttpGet("/gcp/{secretName}")]
    public async Task<IActionResult> GetSecret(string secretName)
    {
        try
        {
            // 设置要访问的 secret 资源名称
            SecretVersionName secretVersionName = new SecretVersionName(_projectId, secretName, "latest");

            // 访问 secret 并获取其值
            AccessSecretVersionResponse result = await _client.AccessSecretVersionAsync(secretVersionName);
            string secretValue = result.Payload.Data.ToStringUtf8();

            return Ok(secretValue);
        }
        catch (Exception ex)
        {
            // 返回错误信息
            return StatusCode(500, $"Error accessing secret: {ex.Message}");
        }
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost(Name = "PostWeatherForecast")]
    public IEnumerable<WeatherForecast> Post(string name)
    {
        _ = name;
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
