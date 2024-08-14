using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly LarkApiClient _larkApiClient;

    public string Records { get; private set; }
    public string AuthUrl { get; private set; }  // URL xác thực Lark Suite

    public IndexModel(LarkApiClient larkApiClient)
    {
        _larkApiClient = larkApiClient;
    }

    public async Task OnGetAsync(string authorizationCode)
    {
        // Xây dựng URL xác thực Lark Suite
        var appId = "cli_a63339047a399010";
        var redirectUri = "https://open.larksuite.com/api-explorer/loading";
        var state = "RANDOMSTATE";  // Thay đổi state theo yêu cầu của bạn

        AuthUrl = $"https://open.larksuite.com/open-apis/authen/v1/index?app_id={appId}&redirect_uri={redirectUri}&state={state}";

        try
        {
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                // Lấy records nếu authorizationCode có sẵn
                Records = await _larkApiClient.GetRecordsAsync(authorizationCode);
            }
        }
        catch (HttpRequestException ex)
        {
            Records = $"Error: {ex.Message}";
        }
    }
}
