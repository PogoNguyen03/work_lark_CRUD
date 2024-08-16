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
    private readonly string _appToken;
    private readonly string _tableId;
    private readonly string _appId;
    private readonly string _appSecret;
    private readonly string _redirectUri;
    private string _accessToken;

    public LarkApiClient(string appId, string appSecret, string redirectUri, string appToken, string tableId)
    {
        _httpClient = new HttpClient();
        _appId = appId;
        _appSecret = appSecret;
        _redirectUri = redirectUri;
        _appToken = appToken;
        _tableId = tableId;
    }

    public async Task EnsureTokenAsync(string authorizationCode)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await GetUserAccessTokenAsync(authorizationCode);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
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
        var requestUri = "https://open.larksuite.com/open-apis/im/v1/messages";

        var data = new
        {
            receive_id = receiveId,
            receive_id_type = receiveIdType,
            content = new
            {
                text = messageContent
            },
            msg_type = "text"
        };

        var requestContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = requestContent
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "t-g2068g6GSUFIJRQ5J3O6YOFQD2KCWOWKPIUBGL7T");

        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Log hoặc in ra chi tiết lỗi
        Console.WriteLine($"Status Code: {response.StatusCode}");
        Console.WriteLine($"Response Body: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
        }
    }

}
