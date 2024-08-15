﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class IndexModel : PageModel
{
    private readonly LarkApiService _larkApiService;

    public string Records { get; private set; }

    public IndexModel(LarkApiService larkApiService)
    {
        _larkApiService = larkApiService;
    }

    public async Task<IActionResult> OnGetAsync(string authorizationCode)
    {
        if (string.IsNullOrEmpty(authorizationCode))
        {
            _larkApiService.RedirectToGetCodeToken(); // Gọi phương thức chính xác
            return new EmptyResult(); // Kết thúc quá trình xử lý sau khi chuyển hướng
        }

        try
        {
            await _larkApiService.GetUserAccessTokenAsync(authorizationCode);
            Records = await _larkApiService.SelectAllBaseAsync();
        }
        catch (HttpRequestException ex)
        {
            Records = $"Error: {ex.Message}";
        }

        return Page();
    }



}
