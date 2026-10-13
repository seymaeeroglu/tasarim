using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TeletipAsistani.Web.Tests;

// WebApplicationFactory uygulamayı bellekte gerçek HTTP hattıyla ayağa kaldırır (entegrasyon testi).
public class HealthEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Health_WhenAppIsRunning_ReturnsHealthy()
    {
        // Given: uygulama çalışıyor
        var client = factory.CreateClient();

        // When: /health isteniyor
        var response = await client.GetAsync("/health");

        // Then: 200 ve "Healthy"
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
