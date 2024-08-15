var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register HttpClient for LarkApiService
builder.Services.AddHttpClient<LarkApiService>(client =>
{
    client.BaseAddress = new Uri("https://open.larksuite.com");
});

// Register the LarkApiService
builder.Services.AddSingleton<LarkApiService>();

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
