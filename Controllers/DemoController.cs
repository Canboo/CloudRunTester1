using Microsoft.AspNetCore.Mvc;
using Google.Cloud.SecretManager.V1;


namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class GCPController : ControllerBase
{
    public GCPController()
    {
        _client = SecretManagerServiceClient.Create();
    }

    private readonly SecretManagerServiceClient _client;

    /// <summary>嘗試取出 Secret Manager 的設定值</summary>
    /// <param name="projectId">請輸入專案Id</param>
    /// <param name="secretName">請輸入密鑰名稱</param>
    /// <response code="204">新增成功</response>
    [HttpGet("/SecretManager/{projectId}/{secretName}")]
    public async Task<IActionResult> GetSecret(string projectId, string secretName)
    {
        try
        {
            // 设置要访问的 secret 资源名称
            SecretVersionName secretVersionName = new SecretVersionName(projectId, secretName, "latest");

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
}
