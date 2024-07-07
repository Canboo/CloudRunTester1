using Microsoft.AspNetCore.Mvc;
using Google.Cloud.SecretManager.V1;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class GCPController : ControllerBase
{
    private readonly ApiContext db;
    public GCPController(ApiContext db)
    {
        _client = SecretManagerServiceClient.Create();
        this.db = db;
    }

    private readonly SecretManagerServiceClient _client;

    /// <summary>嘗試取出 Secret Manager 的設定值</summary>
    /// <param name="projectId">請輸入專案Id</param>
    /// <param name="secretName">請輸入密鑰名稱</param>
    /// <response code="204">新增成功</response>
    [HttpGet("/SecretManager/{projectId}/{secretName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSecret(string projectId, string secretName)
    {
        try
        {
            SecretVersionName secretVersionName = new(projectId, secretName, "latest");
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

    /// <summary>嘗試取出 Cloud SQL For MSSQL 資料表</summary>
    /// <response code="204">新增成功</response>
    [HttpGet("/CloudSQL")]
    public async Task<ActionResult<IEnumerable<TaiwanCity>>> GetSQL()
    {
        return await db.TaiwanCities.ToListAsync();
    }
}
