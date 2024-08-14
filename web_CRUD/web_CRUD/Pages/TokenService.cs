using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _appSecret;
    private string _tenantAccessToken;
    private DateTime _tokenExpiry;

    public TokenService(string appId, string appSecret)
    {
        _httpClient = new HttpClient();
        _appId = appId;
        _appSecret = appSecret;
    }

    private string GetTenantAccessTokenEndpoint() =>
        "https://open.larksuite.com/open-apis/auth/v3/tenant_access_token/internal/";

    public async Task<string> GetTenantAccessTokenAsync()
    {
        if (_tenantAccessToken == null || DateTime.UtcNow >= _tokenExpiry)
        {
            var response = await _httpClient.PostAsync(GetTenantAccessTokenEndpoint(), new StringContent(
                $"{{\"app_id\":\"{_appId}\",\"app_secret\":\"{_appSecret}\"}}",
                Encoding.UTF8, "application/json"
            ));

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JObject.Parse(json);
            _tenantAccessToken = tokenResponse["tenant_access_token"]?.ToString();

            _tokenExpiry = DateTime.UtcNow.AddHours(1);
        }

        Debug.WriteLine($"Tenant Access Token: {_tenantAccessToken}");
        return _tenantAccessToken;
    }

    public async Task<string> GetUserAccessTokenAsync(string authorizationCode)
    {
        try
        {
            var tenantAccessToken = await GetTenantAccessTokenAsync();
            var requestUri = "https://open.larksuite.com/open-apis/auth/v3/user_access_token/";

            var requestBody = new
            {
                app_id = _appId,
                app_secret = _appSecret,
                code = authorizationCode  // Thêm mã xác thực vào yêu cầu
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tenantAccessToken);

            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseBody);

            var userAccessToken = responseJson["user_access_token"]?.ToString();
            if (string.IsNullOrEmpty(userAccessToken))
            {
                throw new KeyNotFoundException("Không tìm thấy khóa 'user_access_token' trong phản hồi.");
            }

            Debug.WriteLine($"User Access Token: {userAccessToken}");
            return userAccessToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserAccessTokenAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<string> ExampleUsageAsync(string authorizationCode)
    {
        var larkApiClient = new LarkApiClient(this, "JZrLbU4i5aRsOKsY0HVlNnRfgTg", "tblJdJc0Z2aecwva");

        try
        {
            var userAccessToken = await GetUserAccessTokenAsync(authorizationCode);
            var records = await larkApiClient.GetRecordsAsync(authorizationCode);
            Console.WriteLine("Records: " + records);
            return records;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
