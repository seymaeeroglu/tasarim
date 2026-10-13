using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TeletipAsistani.Infrastructure;

public static class DependencyInjection
{
    // EF Core DbContext (M1), dosya deposu ve Stub adapter'lar burada kaydedilir.
    // Web'in Infrastructure'a tek temas noktası budur (composition root, bkz. AGENTS.md §5).
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
