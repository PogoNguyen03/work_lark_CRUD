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
        // Đảm bảo có accessToken
        await _larkApiClient.EnsureTokenAsync(code);

        // Tạo bản ghi bằng accessToken đã được thiết lập
        var recordId = await _larkApiClient.CreateRecordAsync(JsonContent, code);

        // Gửi tin nhắn cho người nhận
        string receiveId = "ou_9dc8cf1b8e5535ca825d38ad6bfa12b9";
        string receiveIdType = "user_id"; // Hoặc "open_id" tùy thuộc vào loại ID
        string messageContent = "Bạn đã tạo bản ghi với ID: recpk07ZOz.";

        await _larkApiClient.SendMessageAsync(receiveId, receiveIdType, messageContent);

        return RedirectToPage("Index");
    }
}
