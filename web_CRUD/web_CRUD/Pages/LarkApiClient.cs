using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class LarkApiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://open.larksuite.com/open-apis/bitable/v1/apps/";
    private readonly string _appToken; // The app token or ID
    private readonly string _tableId; // The table ID
    private readonly string _appId; // App ID
    private readonly string _appSecret; // App Secret
    private readonly string _redirectUri; // Redirect URI

    // Constructor with appId, appSecret, redirectUri
    public LarkApiClient(string appId, string appSecret, string redirectUri, string appToken, string tableId)
    {
        _httpClient = new HttpClient();
        _appId = appId;
        _appSecret = appSecret;
        _redirectUri = redirectUri;
        _appToken = appToken;
        _tableId = tableId;
    }

    private async Task EnsureTokenAsync(string authorizationCode)
    {
        var accessToken = await GetUserAccessTokenAsync(authorizationCode);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private string GetEndpoint() => $"{BaseUrl}{_appToken}/tables/{_tableId}/records";

    public async Task<string> GetUserAccessTokenAsync(string authorizationCode)
    {
        var requestUri = "https://open.larksuite.com/open-apis/authen/v1/access_token";

        var data = new
        {
            grant_type = "authorization_code",
            code = authorizationCode,
            app_id = _appId, // App ID của bạn
            app_secret = _appSecret, // App Secret của bạn
            redirect_uri = _redirectUri // Redirect URI đã cấu hình
        };

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(requestUri, content);

        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonDoc = JObject.Parse(jsonResponse);

        // Lấy access token từ phản hồi
        var accessToken = jsonDoc["data"]?["access_token"]?.ToString();

        return accessToken;
    }

    public async Task<string> GetRecordsAsync(string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var response = await _httpClient.GetAsync(GetEndpoint());

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CreateRecordAsync(string jsonContent, string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(GetEndpoint(), content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error creating record: {response.StatusCode} - {errorMessage}");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }


    public async Task<string> UpdateRecordAsync(string recordId, string jsonContent, string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{GetEndpoint()}/{recordId}", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task DeleteRecordAsync(string recordId, string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var response = await _httpClient.DeleteAsync($"{GetEndpoint()}/{recordId}");
        response.EnsureSuccessStatusCode();
    }
}
