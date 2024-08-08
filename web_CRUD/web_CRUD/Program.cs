var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register the LarkApiClient with the required parameters
builder.Services.AddSingleton(sp => new LarkApiClient(
    "JZrLbU4i5aRsOKsY0HVlNnRfgTg",
    "tblJdJc0Z2aecwva",
    "u-caQ9GT66l7qHWoG_xICiEiQh11fh1hd3oyw0g1s02Bog"
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
