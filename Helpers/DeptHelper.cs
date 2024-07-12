using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace api.Helpers;

public class DeptHelper(HttpClient httpClient, IOptions<PremSystem> settings)
{
    public HttpClient Client { get; private set; } = httpClient;
    private readonly IOptions<PremSystem> settings = settings;
    private readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    /// <summary>
    /// 建立連線
    /// </summary>
    public async Task<DeptResult> Create(DeptQuery query)
    {
        return await SendPost(query);
    }

    /// <summary>
    /// HTTP傳送
    /// </summary>
    private async Task<DeptResult> SendPost(DeptQuery entity)
    {
        var req = new StringContent(JsonConvert.SerializeObject(entity, JsonSettings), Encoding.UTF8, "application/json");

        var uri = new Uri(settings.Value.DeptDomain);
        if (Client.BaseAddress != uri) Client.BaseAddress = uri;

        var data = await Client.PostAsync("", req);
        try
        {
            return data.Content.ReadFromJsonAsync<DeptResult>().Result;
        }
        catch (Exception ex)
        {
            return new DeptResult
            {
                Count = 0,
                Error = ex.Message
            };
        }
    }
}
