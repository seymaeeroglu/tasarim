using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.WebEncoders;
using TeletipAsistani.Application;
using TeletipAsistani.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// "/health" yalnızca sürecin ayakta olduğunu söyler; "hazır mı" (/ready) ayrı kavramdır (AGENTS.md §11).
builder.Services.AddHealthChecks();

// Razor varsayılan olarak ı, ş, ğ gibi harfleri &#x131; biçiminde kodlar. Türkçe harfleri
// düz yazdırmasına izin veriyoruz; < > " gibi tehlikeli karakterler yine kodlanır (XSS koruması sürer).
builder.Services.Configure<WebEncoderOptions>(options =>
    options.TextEncoderSettings = new TextEncoderSettings(
        UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement, UnicodeRanges.LatinExtendedA));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
