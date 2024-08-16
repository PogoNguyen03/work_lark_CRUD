using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class UpdateModel : PageModel
{
    private readonly LarkApiClient _larkApiClient;

    [BindProperty]
    public string RecordId { get; set; }
    [BindProperty]
    public string JsonContent { get; set; }

    public UpdateModel(LarkApiClient larkApiClient)
    {
        _larkApiClient = larkApiClient;
    }

    // Phương thức này sẽ hiển thị trang cập nhật mà không cần lấy RecordId từ URL
    public void OnGet()
    {
        // Nếu bạn cần khởi tạo dữ liệu nào đó hoặc chỉ cần hiển thị trang
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var code = Request.Form["code"];
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Cập nhật bản ghi dựa trên RecordId và JsonContent đã nhập
            await _larkApiClient.UpdateRecordAsync(RecordId, JsonContent, code);
            TempData["SuccessMessage"] = "Record updated successfully.";
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            return Page();
        }

        return RedirectToPage("Index"); // Hoặc trang khác sau khi cập nhật thành công
    }
}
