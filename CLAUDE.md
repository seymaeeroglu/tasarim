# CLAUDE.md — Kişiselleştirilmiş Teletıp Asistanı

Claude Code bu dosyayı her oturumun başında okur. Proje kurallarının tamamı tek kaynakta, `AGENTS.md` içindedir; burada tekrar etmiyoruz:

@AGENTS.md

## Claude Code'a özel çalışma kuralları

- Büyük görevlerde önce **plan modunda** çalış: planı sun, onayımı bekle, sonra kodla.
- Her seferinde tek bir küçük görev yap. Bütün projeyi tek adımda yazma.
- Komut çalıştırdığında (build, test, run) sonucunu raporla. Çalıştırmadığın şeyi "çalışıyor" deme.
- Bir dosyayı silmeden, paket eklemeden veya mimariyi değiştirmeden önce gerekçeni söyle ve onay iste.
- `AGENTS.md` ile benim isteğim çelişirse dur ve sor.
- Ben 4. sınıf öğrencisiyim ve projeyi savunacağım: kodun yanında kısa, Türkçe "neden böyle yaptım" açıklaması ver.
- Oturum uzadığında bağlamı şişirme; görev bitince bana kısa bir özet ver (ne yapıldı, ne kaldı, sıradaki görev). Özeti `docs/DURUM.md` dosyasında güncel tut.

## Sık kullanılan komutlar (iskelet kurulunca güncelle)

- Derle: `dotnet build`
- Test: `dotnet test`
- Çalıştır: `dotnet run --project src/TeletipAsistani.Web`
