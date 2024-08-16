//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using System.Threading.Tasks;
//using web_CRUD.Pages.Model;

//public class UpdateModel : PageModel
//{
//    private readonly LarkApiClient _larkApiClient;

//    [BindProperty]
//    public string JsonContent { get; set; }
//    [BindProperty]
//    public string RecordId { get; set; }


//    public UpdateModel(LarkApiClient larkApiClient)
//    {
//        _larkApiClient = larkApiClient;
//    }

//    public void OnGet()
//    {

//    }

//    public async Task<IActionResult> OnPostAsync(string authorizationCode)
//    {
//        if (!ModelState.IsValid)
//        {
//            return Page();
//        }

//        await _larkApiClient.UpdateRecordAsync(RecordId, JsonContent, authorizationCode);
//        return RedirectToPage("Index");
//    }
//}
