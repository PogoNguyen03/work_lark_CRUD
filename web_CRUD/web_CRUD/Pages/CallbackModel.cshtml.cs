using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class CallbackModel : PageModel
{
    private readonly LarkApiService _larkApiService;

    public string Records { get; private set; }

    public CallbackModel(LarkApiService larkApiService)
    {
        _larkApiService = larkApiService;
    }

    public async Task<IActionResult> OnGetAsync(string authorizationCode)
    {
        if (string.IsNullOrEmpty(authorizationCode))
        {
            return BadRequest("Authorization code is missing.");
        }

        try
        {
            // Lấy token người dùng
            await _larkApiService.GetUserAccessTokenAsync(authorizationCode);

            // Lấy tất cả records
            Records = await _larkApiService.SelectAllBaseAsync();
        }
        catch (HttpRequestException ex)
        {
            Records = $"Error: {ex.Message}";
        }

        // Chuyển hướng đến trang chủ hoặc bất kỳ trang nào bạn muốn
        return RedirectToPage("/Index", new { records = Records });
    }

}
