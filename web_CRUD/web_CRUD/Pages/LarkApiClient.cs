using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class LarkApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;
    private const string BaseUrl = "https://open.larksuite.com/open-apis/bitable/v1/apps/";
    private readonly string _appToken;
    private readonly string _tableId;
    private readonly string _appId;
    private readonly string _appSecret;
    private readonly string _redirectUri;
    private string _accessToken;

    public LarkApiClient(string appId, string appSecret, string redirectUri, string appToken, string tableId, TokenService tokenService)
    {
        _httpClient = new HttpClient();
        _appId = appId;
        _appSecret = appSecret;
        _redirectUri = redirectUri;
        _appToken = appToken;
        _tableId = tableId;
        _tokenService = tokenService;
    }

    public async Task EnsureTokenAsync(string authorizationCode)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await GetUserAccessTokenAsync(authorizationCode);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
    }

    private async Task TenantTokenAsync()
    {
        var tenantToken = await _tokenService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenantToken);
    }

    private async Task<string> GetUserAccessTokenAsync(string authorizationCode)
    {
        var requestUri = "https://open.larksuite.com/open-apis/authen/v1/access_token";

        var data = new
        {
            grant_type = "authorization_code",
            code = authorizationCode,
            app_id = _appId,
            app_secret = _appSecret,
            redirect_uri = _redirectUri
        };

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(requestUri, content);

        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonDoc = JObject.Parse(jsonResponse);

        return jsonDoc["data"]?["access_token"]?.ToString();
    }

    public string GetEndpoint() => $"{BaseUrl}{_appToken}/tables/{_tableId}/records";

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


    public async Task<string> DeleteRecordAsync(string recordId, string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var response = await _httpClient.DeleteAsync($"{GetEndpoint()}/{recordId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task SendMessageAsync(string receiveId, string receiveIdType, string messageContent)
    {
        var requestUri = $"https://open.larksuite.com/open-apis/im/v1/messages?receive_id_type={receiveIdType}";

        var data = new
        {
            receive_id = receiveId,
            content = JsonConvert.SerializeObject(new { text = messageContent }),
            msg_type = "text"
        };

        var requestContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        // Get the tenant access token from TokenService
        var tenantToken = await _tokenService.GetAccessTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = requestContent
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tenantToken);

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Log or print the response for debugging
        Debug.WriteLine($"Status Code: {response.StatusCode}");
        Debug.WriteLine($"Response Body: {responseBody}");

        response.EnsureSuccessStatusCode();
    }
}
