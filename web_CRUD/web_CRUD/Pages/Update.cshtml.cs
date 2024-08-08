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

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _larkApiClient.UpdateRecordAsync(RecordId, JsonContent);
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, "Error updating record: " + ex.Message);
            return Page();
        }

        return RedirectToPage("Index");
    }
}
