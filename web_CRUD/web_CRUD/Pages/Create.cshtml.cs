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

    public async Task<IActionResult> OnPostAsync(string authorizationCode)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _larkApiClient.CreateRecordAsync(JsonContent, authorizationCode);
        return RedirectToPage("Index");
    }
}
