# Kişiselleştirilmiş Teletıp Asistanı

SAÜ Bilgisayar Mühendisliği tasarım projesi. KBB ve göğüs hastalıkları şikâyetlerini toplayan, doktorun inceleyip hastaya geri döndüğü bir **eğitim/araştırma prototipi**. Tanı koymaz, ilaç önermez; yalnızca sentetik veriyle çalışır.

Proje kuralları: [AGENTS.md](AGENTS.md) · Güncel durum: [docs/DURUM.md](docs/DURUM.md) · Kullanılan kaynaklar ve lisanslar: [docs/KAYNAKLAR.md](docs/KAYNAKLAR.md)

## Gereksinimler

- .NET SDK 10.0.401 veya üstü 10.0.x (sürüm `global.json` ile sabitlenmiştir)

## Kurulum ve çalıştırma

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/TeletipAsistani.Web
```

Uygulama açıldığında konsolda yazan adrese gidin. Sağlık kontrolü: `/health` → `Healthy`.

## Yapı

```
src/
  TeletipAsistani.Web/            MVC: Controller, View (sunum katmanı)
  TeletipAsistani.Application/    kullanım senaryoları, arayüzler
  TeletipAsistani.Domain/         varlıklar ve iş kuralları (hiçbir şeye bağımlı değil)
  TeletipAsistani.Infrastructure/ veritabanı, dosya, dış servis adapter'ları
tests/                            xUnit birim ve entegrasyon testleri
docs/                             durum ve kaynak kayıtları
```

Bağımlılık yönü `tests/TeletipAsistani.Web.Tests/ArchitectureTests.cs` ile otomatik denetlenir.
