using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TeletipAsistani.Web.Tests;

public class HomePageTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task HomePage_ShowsNoDiagnosisAndEmergencyNotice()
    {
        // Given: uygulama çalışıyor
        var client = factory.CreateClient();

        // When: ana sayfa açılıyor
        var response = await client.GetAsync("/");
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        // Then: AGENTS.md §9'daki zorunlu uyarılar sayfada görünür
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Bu sistem tanı koymaz", html);
        Assert.Contains("112", html);
    }
}
