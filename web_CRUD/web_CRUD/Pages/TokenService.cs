using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly string _appSecret;
    private string _accessToken;
    private DateTime _tokenExpiry;

    public TokenService(string appId, string appSecret)
    {
        _httpClient = new HttpClient();
        _appId = appId;
        _appSecret = appSecret;
    }

    private string GetTokenEndpoint() =>
        "https://open.larksuite.com/open-apis/auth/v3/tenant_access_token/internal/";

    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken == null || DateTime.UtcNow >= _tokenExpiry)
        {
            var response = await _httpClient.PostAsync(GetTokenEndpoint(), new StringContent(
                $"{{\"app_id\":\"{_appId}\",\"app_secret\":\"{_appSecret}\"}}",
                Encoding.UTF8, "application/json"
            ));

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JObject.Parse(json);
            _accessToken = tokenResponse["tenant_access_token"]?.ToString();

            // Assume the token is valid for 1 hour
            _tokenExpiry = DateTime.UtcNow.AddHours(1);
        }

        return _accessToken;
    }
}
