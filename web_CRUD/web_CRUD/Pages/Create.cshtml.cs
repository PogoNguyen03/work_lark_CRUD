using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class CreateModel : PageModel
{
    private readonly LarkApiClient _larkApiClient;

    [BindProperty]
    public string JsonContent { get; set; }

    public CreateModel(LarkApiClient larkApiClient)
    {
        _larkApiClient = larkApiClient;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Lấy authorizationCode từ query string hoặc form (cần điều chỉnh tùy theo cách bạn nhận code)
        var code = Request.Form["code"];

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _larkApiClient.EnsureTokenAsync(code);

        // Tạo bản ghi bằng accessToken đã được thiết lập
        await _larkApiClient.CreateRecordAsync(JsonContent, code);

        return RedirectToPage("Index");
    }
}
