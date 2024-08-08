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
    private readonly string _authorization;

    public LarkApiClient(string appToken, string tableId, string authorization)
    {
        _httpClient = new HttpClient();
        _appToken = appToken;
        _tableId = tableId;
        _authorization = authorization;

        // Ensure the Authorization header is set
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorization);
    }

    private string GetEndpoint() => $"{BaseUrl}{_appToken}/tables/{_tableId}/records";

    public async Task<string> GetRecordsAsync()
    {
        var response = await _httpClient.GetAsync(GetEndpoint());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CreateRecordAsync(string jsonContent)
    {
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(GetEndpoint(), content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> UpdateRecordAsync(string recordId, string jsonContent)
    {
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Patch, $"{GetEndpoint()}/{recordId}")
        {
            Content = content
        };

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            // Log the error for debugging purposes
            Console.WriteLine($"Request to update record failed: {ex.Message}");
            Console.WriteLine($"Endpoint: {request.RequestUri}");
            Console.WriteLine($"Content: {jsonContent}");
            throw;
        }
    }

    public async Task DeleteRecordAsync(string recordId)
    {
        var response = await _httpClient.DeleteAsync($"{GetEndpoint()}/{recordId}");
        response.EnsureSuccessStatusCode();
    }
}
