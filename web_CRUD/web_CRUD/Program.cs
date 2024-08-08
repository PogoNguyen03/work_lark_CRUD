var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the TokenService and LarkApiClient
builder.Services.AddSingleton<TokenService>(sp =>
    new TokenService("your_app_id", "your_app_secret"));

builder.Services.AddSingleton<LarkApiClient>(sp =>
    new LarkApiClient(
        sp.GetRequiredService<TokenService>(),
        "JZrLbU4i5aRsOKsY0HVlNnRfgTg",  // App token
        "tblJdJc0Z2aecwva"  // Table ID
    ));

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
