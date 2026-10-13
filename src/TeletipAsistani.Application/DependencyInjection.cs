using Microsoft.Extensions.DependencyInjection;

namespace TeletipAsistani.Application;

public static class DependencyInjection
{
    // Uygulama servisleri (SubmitComplaintService vb.) M3'ten itibaren burada kaydedilir.
    // Web katmanı servisleri tek tek bilmez; yalnızca bu metodu çağırır.
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
