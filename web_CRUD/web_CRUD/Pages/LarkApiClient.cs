using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // for accessing cookies
using Microsoft.Extensions.Configuration; // for app settings

public class LarkApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    private readonly string TableId = "tblJdJc0Z2aecwva";
    private readonly string AppToken = "JZrLbU4i5aRsOKsY0HVlNnRfgTg";
    private readonly string DepartmentId = "0";
    private readonly string RedirectUri = "https://open.larksuite.com/api-explorer/loading";
    private readonly string RedirectUriCallBack = "https://open.larksuite.com/api-explorer/loading/callback"; // Cập nhật URL callback của bạn


    public LarkApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;

        // Default headers
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private string UserToken => _httpContextAccessor.HttpContext.Request.Cookies["user_access_token"];

    private string BaseUrl => $"/api/open-apis/bitable/v1/apps/{AppToken}/tables/{TableId}/records";

    private static readonly (string AppId, string AppSecret) AppSecretConfig = (
        "cli_a63339047a399010",
        "b97g18wMrejUvbdhGRemagTZAI2RST5r"
    );

    public void RedirectToGetCodeToken()
    {
        try
        {
            var redirectUrl = $"https://open.larksuite.com/open-apis/authen/v1/index?redirect_uri={RedirectUri}&app_id={AppSecretConfig.AppId}&state=RANDOMSTATE";
            _httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting code token: {ex.Message}");
        }
    }


    public async Task<string> GetAppAccessTokenAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync(
                "/api/open-apis/auth/v3/app_access_token/internal",
                new StringContent(JsonSerializer.Serialize(new { app_id = AppSecretConfig.AppId, app_secret = AppSecretConfig.AppSecret }), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            return jsonDoc.RootElement.GetProperty("app_access_token").GetString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting app access token: {ex.Message}");
            throw;
        }
    }


    public async Task<string> GetUserAccessTokenAsync(string authorizationCode)
    {
        try
        {
            var data = new
            {
                grant_type = "authorization_code",
                code = authorizationCode,
                app_id = AppSecretConfig.AppId,
                app_secret = AppSecretConfig.AppSecret,
                redirect_uri = RedirectUri
            };

            var response = await _httpClient.PostAsync(
                "/api/open-apis/authen/v1/access_token",
                new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var accessToken = jsonDoc.RootElement.GetProperty("data").GetProperty("access_token").GetString();

            // Set the user access token in a cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append("user_access_token", accessToken, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddHours(1) });

            // Redirect to home page
            _httpContextAccessor.HttpContext.Response.Redirect("/");

            return accessToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user access token: {ex.Message}");
            throw;
        }
    }

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(UserToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserToken);
        }
    }

    public async Task<string> SelectAllBaseAsync()
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting all base: {ex.Message}");
            throw;
        }
    }

    public async Task<string> CreateTaskBaseAsync(object fields)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsync(BaseUrl, new StringContent(JsonSerializer.Serialize(fields), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating task base: {ex.Message}");
            throw;
        }
    }

    public async Task<string> UpdateTaskBaseAsync(object data, string key)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsync($"{BaseUrl}/{key}", new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task base: {ex.Message}");
            throw;
        }
    }

    public async Task<string> DeleteTaskBaseAsync(string key)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{key}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task base: {ex.Message}");
            throw;
        }
    }

    public async Task<string> SelectAllUserAsync()
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/open-apis/contact/v3/users/find_by_department?department_id={DepartmentId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error selecting all users: {ex.Message}");
            throw;
        }
    }
}
