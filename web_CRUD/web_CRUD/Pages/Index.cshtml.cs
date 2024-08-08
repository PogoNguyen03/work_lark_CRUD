using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly LarkApiClient _larkApiClient;

    public string Records { get; private set; }

    public IndexModel(LarkApiClient larkApiClient)
    {
        _larkApiClient = larkApiClient;
    }

    public async Task OnGetAsync()
    {
        try
        {
            Records = await _larkApiClient.GetRecordsAsync();
        }
        catch (HttpRequestException ex)
        {
            Records = $"Error: {ex.Message}";
        }
    }
}
