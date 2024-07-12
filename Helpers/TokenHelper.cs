using Google.Apis.Auth.OAuth2;

namespace api.Helpers;

public class TokenHelper
{
    /// <summary>
    /// 取得 gcloud token
    /// </summary>
    public async Task<TokenResult> Create(string audience)
    {
        GoogleCredential credential;
        try
        {
            credential = await GoogleCredential.GetApplicationDefaultAsync();
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(["https://www.googleapis.com/auth/cloud-platform"]);
            }
        }
        catch (Exception ex)
        {
            return new TokenResult("", $"Failed to get Google credentials: {ex.Message}");
        }
        string token;
        try
        {
            OidcToken ocidToken = await credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(audience));
            token = await ocidToken.GetAccessTokenAsync();
        }
        catch (Exception ex)
        {
            return new TokenResult("", $"Failed to get access token: {ex.Message}");
        }

        return new TokenResult(token, "");
    }
}

public class TokenResult(string token, string error)
{
    public string Token { get; set; } = token;
    public string Error { get; set; } = error;
}
