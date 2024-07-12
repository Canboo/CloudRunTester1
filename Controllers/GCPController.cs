using Microsoft.AspNetCore.Mvc;
using Google.Cloud.SecretManager.V1;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Google.Cloud.Storage.V1;
using Google.Cloud.Functions.V2;
using Google.Api.Gax.ResourceNames;
using Google.LongRunning;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class GCPController(ApiContext db, IConfiguration configuration, DeptHelper publisher) : ControllerBase
{
    private readonly FunctionServiceClient _function = FunctionServiceClient.Create();
    private readonly IConfiguration _configuration = configuration;
    private readonly ApiContext db = db;
    private readonly DeptHelper publisher = publisher;
    private readonly SecretManagerServiceClient _client = SecretManagerServiceClient.Create();
    private readonly StorageClient _storageClient = StorageClient.Create();

    /// <summary>嘗試取出 Secret Manager 的設定值</summary>
    [HttpGet("/Functions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFunctions()
    {
        try
        {
            FunctionServiceClient functionServiceClient = await FunctionServiceClient.CreateAsync();
            FunctionName parent = new("668790833830", "asia-east1", "pocfunctiondep");
            Function response = functionServiceClient.GetFunction(parent);

            return Ok(response);
        }
        catch (Exception ex)
        {
            // 返回错误信息
            return StatusCode(500, $"Error accessing secret: {ex.Message}");
        }
    }

    /// <summary>嘗試取出 Secret Manager 的設定值</summary>
    /// <param name="projectId">請輸入專案Id</param>
    /// <param name="secretName">請輸入密鑰名稱</param>
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
    [HttpGet("/CloudSQL")]
    public async Task<ActionResult<IEnumerable<TaiwanCity>>> GetSQL()
    {
        return await db.TaiwanCities.ToListAsync();
    }

    /// <summary>嘗試取出 GCS 檔案</summary>
    /// <param name="filename">輸入要取得的檔案名稱</param>
    /// <response code="204">新增成功</response>
    [HttpGet("/GCS/{filename}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGCS(string filename)
    {
        string projectId = _configuration["GCP:ProjectId"];
        try
        {
            SecretVersionName secretVersionName = new(projectId, "gcs-name", "latest");
            AccessSecretVersionResponse result = await _client.AccessSecretVersionAsync(secretVersionName);
            string secretValue = result.Payload.Data.ToStringUtf8();

            var memoryStream = new MemoryStream();
            await _storageClient.DownloadObjectAsync(secretValue, filename, memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream, "application/octet-stream", filename);
        }
        catch (Google.GoogleApiException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>嘗試透過 Cloud Functions 取出地端 vw_StuAffairs_sysDep 資料表</summary>
    [HttpPost("/Depts")]
    public async Task<ActionResult<DeptResult>> GetDepts(DeptQuery query)
    {
        return await publisher.Create(query);
    }
}
