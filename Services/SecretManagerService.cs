// Services/SecretManagerService.cs
using Google.Cloud.SecretManager.V1;
using System.Threading.Tasks;

public class SecretManagerService
{
    private readonly SecretManagerServiceClient _client;

    public SecretManagerService()
    {
        _client = SecretManagerServiceClient.Create();
    }

    public async Task<string> GetSecretAsync(string projectId, string secretName)
    {
        var secretVersionName = new SecretVersionName(projectId, secretName, "latest");
        AccessSecretVersionResponse result = await _client.AccessSecretVersionAsync(secretVersionName);
        return result.Payload.Data.ToStringUtf8();
    }

    public async Task<string> GetConnectionStringAsync(string projectId)
    {
        string dbhost = await GetSecretAsync(projectId, "dbhost");
        string dbname = await GetSecretAsync(projectId, "dbname");
        string dbuser = await GetSecretAsync(projectId, "dbuser");
        string dbpassword = await GetSecretAsync(projectId, "dbpassword");

        return $"server={dbhost};Database={dbname};MultipleActiveResultSets=true;TrustServerCertificate=true;UID={dbuser};PWD={dbpassword};";
    }
}
