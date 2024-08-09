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
    private readonly string _appToken; // The app token or ID
    private readonly string _tableId; // The table ID

    public LarkApiClient(TokenService tokenService, string appToken, string tableId)
    {
        _httpClient = new HttpClient();
        _tokenService = tokenService;
        _appToken = appToken;
        _tableId = tableId;
    }

    private async Task EnsureTokenAsync()
    {
        var accessToken = await _tokenService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    private string GetEndpoint() => $"{BaseUrl}{_appToken}/tables/{_tableId}/records";

    public async Task<string> GetRecordsAsync()
    {
        await EnsureTokenAsync();
        var response = await _httpClient.GetAsync(GetEndpoint());

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }


    public async Task<string> CreateRecordAsync(string jsonContent)
    {
        await EnsureTokenAsync();
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(GetEndpoint(), content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }


    public async Task<string> UpdateRecordAsync(string recordId, string jsonContent)
    {
        await EnsureTokenAsync();
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{GetEndpoint()}/{recordId}", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task DeleteRecordAsync(string recordId)
    {
        await EnsureTokenAsync();
        var response = await _httpClient.DeleteAsync($"{GetEndpoint()}/{recordId}");
        response.EnsureSuccessStatusCode();
    }
}
