# Proje Durumu

Son güncelleme: 13 Ekim 2026

## Tamamlanan

### M0 — İskelet (dal: `feat/m0-iskelet`)
- Solution: Web, Application, Domain, Infrastructure + 3 test projesi (.NET 10, `global.json` 10.0.401).
- Ortak ayarlar `Directory.Build.props`: nullable açık, nullable uyarıları hata.
- Composition root: `AddApplication()` ve `AddInfrastructure()` (henüz boş; M1'de dolacak).
- `/health` uç noktası (yerleşik HealthChecks, ek paket yok).
- Türkçe layout; her sayfada "tanı koymaz / 112" uyarısı; teknik ayrıntı göstermeyen Türkçe hata sayfası.
- Testler (6/6 geçti): `/health` 200; ana sayfa uyarıları; katman bağımlılık testi (4 durum).
- Doğrulama seviyesi: birim + entegrasyon (WebApplicationFactory) + elle çalıştırma (`dotnet run`, `/health` ve `/` yanıtı kontrol edildi).

## Bilinen eksikler
- Domain.Tests ve Application.Tests henüz boş (ilk varlıklar M1–M3'te gelecek).
- Güvenlik başlıkları, rate limiting, oturum ayarları M1'de eklenecek.

## Sıradaki görev
**M1 — Kimlik:** ASP.NET Core Identity + EF Core SQLite (Infrastructure'da), roller (Hasta, Doktor, Yönetici), yalnızca hasta kaydı açık formu, yöneticinin doktor hesabı oluşturması, sentetik seed kullanıcılar (yalnızca Development), parola/kilitleme/çerez ayarları.
