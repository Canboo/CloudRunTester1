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

    public async Task<string> GetConnectionStringAsync(string projectId, string host = "")
    {
        var tasks = new List<Task<string>>();

        if (string.IsNullOrEmpty(host))
        {
            tasks.Add(GetSecretAsync(projectId, "dbhost"));
        }
        else
        {
            tasks.Add(Task.FromResult(host));
        }

        tasks.Add(GetSecretAsync(projectId, "dbname"));
        tasks.Add(GetSecretAsync(projectId, "dbuser"));
        tasks.Add(GetSecretAsync(projectId, "dbpassword"));

        var results = await Task.WhenAll(tasks);

        string dbhost = results[0];
        string dbname = results[1];
        string dbuser = results[2];
        string dbpassword = results[3];

        return $"server={dbhost};Database={dbname};MultipleActiveResultSets=true;TrustServerCertificate=true;UID={dbuser};PWD={dbpassword};";
    }
}
