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
        var receiveId = "ou_9dc8cf1b8e5535ca825d38ad6bfa12b9"; // ID người dùng
        var receiveIdType = "open_id"; // Loại ID nhận tin nhắn

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Gửi nội dung JSON làm tin nhắn
            await _larkApiClient.SendMessageAsync(receiveId, receiveIdType, JsonContent);
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, $"Error sending message: {ex.Message}");
            return Page();
        }

        return RedirectToPage("Index");
    }
}
