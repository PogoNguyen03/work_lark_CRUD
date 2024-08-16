using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class DeleteModel : PageModel
{
    private readonly LarkApiClient _larkApiClient;

    [BindProperty]
    public string RecordId { get; set; }

    public DeleteModel(LarkApiClient larkApiClient)
    {
        _larkApiClient = larkApiClient;
    }

    public void OnGet()
    {
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
            await _larkApiClient.DeleteRecordAsync(RecordId, code);
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
