using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly LarkApiClient _larkApiClient;

    public string Records { get; private set; }
    public string ErrorMessage { get; private set; }

    public IndexModel(LarkApiClient larkApiClient)
    {
        _larkApiClient = larkApiClient;
    }

    public async Task OnGetAsync(string code)
    {
        if (!string.IsNullOrEmpty(code))
        {
            try
            {
                // Xử lý mã xác thực và lấy dữ liệu
                Records = await _larkApiClient.GetRecordsAsync(code);
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
        else
        {
            ErrorMessage = "Authorization code is missing.";
        }
    }
}
