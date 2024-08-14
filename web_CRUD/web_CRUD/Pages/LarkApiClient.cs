using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class LarkApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;
    private const string BaseUrl = "https://open.larksuite.com/open-apis/bitable/v1/apps/";
    private readonly string _appToken;
    private readonly string _tableId;

    public LarkApiClient(TokenService tokenService, string appToken, string tableId)
    {
        _httpClient = new HttpClient();
        _tokenService = tokenService;
        _appToken = appToken;
        _tableId = tableId;
    }

    private async Task EnsureTokenAsync(string authorizationCode)
    {
        var userAccessToken = await _tokenService.GetUserAccessTokenAsync(authorizationCode);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);
    }

    private string GetEndpoint() => $"{BaseUrl}{_appToken}/tables/{_tableId}/records";

    public async Task<string> GetRecordsAsync(string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var response = await _httpClient.GetAsync(GetEndpoint());

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

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
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> UpdateRecordAsync(string recordId, string jsonContent, string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{GetEndpoint()}/{recordId}", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task DeleteRecordAsync(string recordId, string authorizationCode)
    {
        await EnsureTokenAsync(authorizationCode);
        var response = await _httpClient.DeleteAsync($"{GetEndpoint()}/{recordId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }
    }
}
