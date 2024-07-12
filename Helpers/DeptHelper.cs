using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace api.Helpers;

public class DeptHelper(HttpClient httpClient, IMemoryCache memoryCache, IOptions<PremSystem> settings)
{
    public HttpClient Client { get; private set; } = httpClient;
    private readonly IOptions<PremSystem> settings = settings;
    private readonly IMemoryCache memoryCache = memoryCache;
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
        string memoryKey = $"PremDept-{entity.DepYear}";
        if (!memoryCache.TryGetValue(memoryKey, out DeptResult data))
        {
            var req = new StringContent(JsonConvert.SerializeObject(entity, JsonSettings), Encoding.UTF8, "application/json");

            var uri = new Uri(settings.Value.DeptDomain);
            if (Client.BaseAddress != uri) Client.BaseAddress = uri;

            var result = await Client.PostAsync("", req);
            try
            {
                data = result.Content.ReadFromJsonAsync<DeptResult>().Result;
                // 寫進緩存
                memoryCache.Set(memoryKey, data, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300)
                });
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

        return data;
    }
}
