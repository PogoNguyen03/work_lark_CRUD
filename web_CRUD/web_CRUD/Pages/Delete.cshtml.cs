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

    public async Task<IActionResult> OnPostAsync(string authorizationCode)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _larkApiClient.DeleteRecordAsync(RecordId, authorizationCode);
        return RedirectToPage("Index");
    }
}
