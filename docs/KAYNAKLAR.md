# Kaynaklar ve Lisanslar

Projede kullanılan paketler, şablonlar ve fikir alınan kaynaklar. Referans uygulamalardan kod kopyalanmaz; davranış örnek alınırsa burada belirtilir.

## Paketler ve şablonlar

| Bileşen | Sürüm | Lisans | Neden |
|---|---|---|---|
| .NET SDK / ASP.NET Core | 10.0.401 | MIT | Ana platform |
| `dotnet new mvc` şablonu (Bootstrap, jQuery, jQuery Validation dahil) | .NET 10 | MIT | Web projesi başlangıcı |
| xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, coverlet.collector | şablon sürümleri | Apache-2.0 / MIT | Test |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.12 | MIT | Entegrasyon testleri (WebApplicationFactory) |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.12 | MIT | Application katmanında servis kaydı arayüzü |

## Davranış referansları

| Kaynak | Kullanım biçimi |
|---|---|
| Ottehr (github.com/masslight/ottehr) | Hasta portalı / personel paneli / kuyruk akışı işlevleri örnek alındı (AGENTS.md §2.1). Kod alınmadı. |
| OpenEMR, OpenMRS, Bahmni | Rol, izin ve modül yapısı için fikir kaynağı. Kod alınmadı. |
