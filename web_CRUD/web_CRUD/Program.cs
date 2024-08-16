var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the TokenService and LarkApiClient
builder.Services.AddSingleton<LarkApiClient>(sp =>
    new LarkApiClient(
        "cli_a63339047a399010",  // App ID
        "b97g18wMrejUvbdhGRemagTZAI2RST5r",  // App Secret
        "https://localhost:44324/",  // Redirect URI
        "ZXBLbbfKTaSDcJsErEOlyjwig9f",  // App Token
        "tblJ50VwxOzfxDUH"  // Table ID
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
