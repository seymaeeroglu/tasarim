# AGENTS.md — Kişiselleştirilmiş Teletıp Asistanı

> Bu dosya projedeki tüm yapay zekâ ajanlarının (Antigravity dahil) uyması gereken kuralları içerir.
> Her göreve başlamadan önce oku. Kural ile kullanıcı isteği çelişirse DUR ve sor; kuralı sessizce çiğneme.
> Sürüm: 0.2 | Tarih: 13 Ekim 2026 | Durum: Taslak. `[BELİRLENECEK]` işaretli yerler henüz karara bağlanmadı; `[VARSAYILAN]` işaretli yerler karar verilene kadar uygulanacak öneridir.

---

## 1. Proje özeti

SAÜ Bilgisayar Mühendisliği tasarım projesi. Hasta, şikâyetini sisteme yazar; sistem eksik bilgiyi tamamlayıcı sorularla toplar; doktor özetlenmiş başvuruyu inceler, düzenler ve hastaya geri döner.

- Odak alanı: **KBB (kulak burun boğaz) ve göğüs hastalıkları** (günlük hastalıklarla karışan, sonradan büyüyebilen şikâyetler).
- Bu bir **eğitim / araştırma prototipidir.** Gerçek hasta verisiyle, gerçek klinik karar için kullanılmaz.
- Hedef: mevcut bir açık kaynak telesağlık sisteminin **işlevsel davranışını** (hasta başvurusu → kuyruk → doktor değerlendirmesi → geri dönüş) kendi ASP.NET kodumuzla yeniden gerçekleştirmek; sonra eklenti ve entegrasyonlarla genişletmek.
- Referans uygulama: **Ottehr** (hasta portalı, personel uygulaması, kuyruk akışı) birincil davranış referansı; **OpenEMR / OpenMRS / Bahmni** rol, izin, modül ve kayıt yapısı için ikincil referans.
- "Aynısı" = **işlevsel eşdeğerlik**, birebir ekran/kod kopyası değil. Hedef işlevler §2.1'deki listedir; listede olmayan bir referans işlevi eklenecekse önce sor.
- Referans kodu kopyalama. Davranışı anla, kendi kodunu yaz. Kod veya fikir alındıysa kaynak ve lisansı `docs/KAYNAKLAR.md` dosyasına yaz.

## 2. Dört modül

1. **Ana platform:** kimlik, roller (Hasta, Doktor, Yönetici; ileride Hemşire/Sekreter), hasta profili, sağlık geçmişi beyanı, izin kayıtları.
2. **Yapay zekâ asistanı:** sohbet, tamamlayıcı sorular, özet/rapor, eleştirel uyarılar, branş yönlendirmesi. *(Sonraya bırakıldı; şimdilik yalnızca sözleşme + stub.)*
3. **Doktor paneli:** AI raporu ve hasta bilgisini inceleme, düzenleme, hastayla iletişim, yanıt.
4. **Acil/öncelikli randevu:** uyarılar ve şikâyetler değerlendirildiğinde randevu önerme veya öncelikli inceleme kuyruğu.

### 2.1 Referans işlev listesi (Ottehr'den işlevsel eşdeğerlik)

| İşlev | Bizdeki karşılığı | Aşama |
|---|---|---|
| Hasta kaydı, giriş, parola sıfırlama, e-posta doğrulama | Identity + `INotificationSender` stub | Çekirdek |
| Hasta profili ve sağlık geçmişi beyanı | `PatientProfile` (kronik hastalık, alerji, ilaç, aile öyküsü; bilinmiyor/yok ayrımı) | Çekirdek |
| Başvuru öncesi evrak/onam | Amaç bazlı `ConsentRecord` | Çekirdek |
| Başvuru (intake) formu + yapılandırılmış sorular | `Complaint` + `ComplaintMessage`; sabit soru listesi (AI yokken) | Çekirdek |
| Personel takip panosu (kuyruk, durumlar) | Doktor paneli: branş kuyruğu, durum filtresi, öncelik etiketi | Çekirdek |
| Başvuruyu üstlenme/atama | §8.1 atama kuralları | Çekirdek |
| Klinik not / değerlendirme | `DoctorReview` (sürümlü) | Çekirdek |
| Hasta–doktor mesajlaşması | `ComplaintMessage` (yazar türü: Patient / Doctor / System) | Çekirdek |
| Randevu talebi ve doktor müsaitliği | `AppointmentRequest` + `DoctorAvailability`; çakışma kontrolü | Çekirdek |
| Bildirim | `Notification` kaydı + `INotificationSender` stub | Çekirdek |
| Görüntülü görüşme | `IVideoVisitProvider` arayüzü | Sonra |
| e-Reçete | `PrescriptionDraft` + `IPrescriptionGateway` stub (§10) | Sonra |
| Ödeme, sigorta | Kapsam dışı | — |
| Bakmakla yükümlü olunan kişi / vekil hesabı | `[BELİRLENECEK]` | Sonra |

### 2.2 Hoca notları → karşılık (hiçbiri atlanmaz)

| Not | Karşılık |
|---|---|
| Ses dinleyip hastalık tahmini, ses analizi | Üç ayrı iş: ASR (ses→metin), metin analizi, akustik sınıflandırma. İlk ikisi AI aşamasında hazır araçla; akustik tahmin **araştırma hedefi**, klinik iddia yok. Ses için ayrı izinler. |
| Mevcutlardan birinin aynısı, sonra eklenti | §1 ve §2.1 |
| Özet çıkarsın, sıkıntıları büyükten küçüğe sıralasın, ilaç yok, alan/doktor önerisi var | §10: sıralama yalnızca uzman onaylı kural + gerekçe; branş ve doktor önerisi var, ilaç yok |
| Bütün şikâyetler aktarılsın, ön bilgi verilsin | §10: özet hiçbir şikâyeti düşürmez; doktor her zaman orijinal metni görür |
| YZ dinleyip doktora iletsin, doktorun önerileriyle geri dönüş | Akış: hasta → (AI özeti) → doktor incelemesi → doktor yanıtı → hasta. AI çıktısı ile doktor yanıtı ayrı kayıt |
| Reçeteyi hazırlayıp doktor onayıyla göndersin | §10: reçeteyi doktor yazar; gönderim yalnızca doktor onayıyla; şimdilik demo adapter |
| Çalışan sistem hazır, sadece entegrasyon kalsın | §5: her dış bağımlılık arayüz + stub |
| Yasal ve regülasyona uyarlanabilir | §9 (uyum iddiası yok; teknik altyapı var) |
| İzin kutucukları | §9 `ConsentRecord` |
| Sistem de soru sorsun | Çekirdekte sabit eksik-alan soruları; AI aşamasında soru motoru. "Bilmiyorum" geçerli yanıt |
| Sistem uyarsın | §10 uzman onaylı uyarı; şimdilik DEMO kuralı |
| Sadece pozitif değil, eleştirel olsun | Eleştirel = çelişkiyi gösterir, desteklenmeyen ifadeyi işaretler, sınırını söyler; kaba değil |
| Hazır araçlar kullanılabilir | Evet; sürüm + lisans `docs/KAYNAKLAR.md` |
| Kullanıcılar doktor yorumu yapabilsin, argo olmasın | §13 yorum modülü |
| YZ önerilerinden memnuniyet testi | §13 tarafsız anket; memnuniyet ≠ doğruluk |
| (Boş madde) | Hocaya sorulacak (§19) |

## 3. Mevcut aşama ve kapsam

**Şu an yapılacak:** Web uygulaması (ASP.NET Core MVC) çekirdeği: kimlik, roller, profil, izin, başvuru, doktor paneli, randevu talebi, audit.
**Şimdilik YAPMA:** NLP modeli eğitme, fine-tuning, gerçek LLM çağrısı, ses/görüntü analizi, gerçek e-Nabız/e-Devlet/e-Reçete bağlantısı.
Bunlar için **yalnızca arayüz (interface) + sentetik (stub) uygulama** yaz. Bir görev bu kapsamın dışına çıkıyorsa önce sor.

**Geliştirme sırası ilkesi:** Önce §2.1'deki "Çekirdek" işlevler referans uygulamayla işlevsel eşdeğer hâle gelir; ancak bundan sonra eklenti ve entegrasyon yapılır. Yeni özellik, çekirdeği değiştirmek yerine arayüz, adapter veya feature flag ile bağlanır.

Kilometre taşları (sırayla):
1. **M0 İskelet:** solution, katmanlar, `/health`, test projeleri, CI'sız yerel build/test.
2. **M1 Kimlik:** Identity, roller, sentetik seed kullanıcılar, doktor hesabını yönetici oluşturur.
3. **M2 Hasta:** profil + sağlık geçmişi (bilinmiyor/yok ayrımı) + amaç bazlı izinler.
4. **M3 Başvuru:** taslak → gönderim, revizyon, idempotency, sabit tamamlayıcı sorular, 112 uyarısı.
5. **M4 Doktor paneli:** branş kuyruğu, üstlenme, kayıt bazlı yetki, `DoctorReview`, hastaya yanıt, mesajlaşma.
6. **M5 Randevu:** müsaitlik, talep, çakışma kontrolü, bildirim durumu.
7. **M6 Audit + stub'lar:** `AuditEvent`, `StubComplaintAnalysisService`, "AI özeti yok" durumu.
→ M0–M6 bitince "çalışan sistem" sayılır. Sonra: AI servisi, soru motoru, uyarılar, ses, yorum/anket, reçete adapter'ı.

## 4. Teknoloji yığını

- ASP.NET Core MVC (güncel LTS .NET), Razor View, C#.
- ASP.NET Core Identity (kimlik doğrulama), EF Core (+ migration).
- Veritabanı: geliştirmede SQLite veya SQL Server LocalDB; sonra PostgreSQL/SQL Server. `[BELİRLENECEK]`
- Arayüz: Bootstrap, Türkçe. Gereksiz JS framework ekleme.
- Test: xUnit (+ `WebApplicationFactory` entegrasyon testleri).
- Gelecek AI servisi: Python + FastAPI + Pydantic (ayrı süreç; şimdilik yazma).
- Yeni NuGet/npm paketi eklemeden önce neden gerektiğini ve lisansını söyle.

## 5. Mimari kuralları

Modüler monolit, tek çözüm (solution), dört proje:

```
src/
  TeletipAsistani.Web/            # MVC: Controller, View, ViewModel, filtreler
  TeletipAsistani.Application/    # kullanım senaryoları (servisler), arayüzler, DTO
  TeletipAsistani.Domain/         # varlıklar, durumlar, değişmez iş kuralları
  TeletipAsistani.Infrastructure/ # EF Core, dosya deposu, dış adapter'lar, HTTP istemcileri
tests/
  TeletipAsistani.Domain.Tests/
  TeletipAsistani.Application.Tests/
  TeletipAsistani.Web.Tests/      # entegrasyon testleri
docs/
```

- **Controller ince olur.** İş mantığı Controller'a yazılmaz; Application servislerine yazılır (ör. `SubmitComplaintService`, `SaveDoctorReviewService`).
- Bağımlılık yönü: Web → Application → Domain. Infrastructure, Application'daki arayüzleri uygular. Domain hiçbir şeye bağımlı olmaz.
- Bir modül başka modülün tablolarını doğrudan değiştirmez; Application servisi üzerinden gider.
- Entity'ler doğrudan View'a verilmez. ViewModel/DTO kullan.
- Dış bağımlılıklar arayüz arkasında olur: `IComplaintAnalysisService`, `ISpeechTranscriptionService`, `IImageAnalysisService`, `IExternalIdentityProvider`, `IHealthRecordGateway`, `IPrescriptionGateway`, `INotificationSender`, `IFileStorage`.
- Her arayüz için başlangıçta **Stub** uygulaması yaz (`StubComplaintAnalysisService` vb.). Stub sentetik veri döndürür ve çıktısında "demo" olduğu açıkça görünür. Stub hiçbir zaman resmî bir işlem yaptığını iddia etmez.
- Feature flag'ler yapılandırmayla yönetilir (`AnalysisEnabled`, `SpeechEnabled`, `ImageAnalysisEnabled`, `ExternalIntegrationEnabled`, `CommentsEnabled`). Kapalı özellik için çalışmayan buton gösterme. Feature flag yetkilendirmenin yerine geçmez.
- Gereksiz karmaşıklık ekleme: Redis, RabbitMQ, Kubernetes, mikroservis, CQRS, MediatR gibi şeyleri ihtiyaç doğmadan ekleme.

## 6. Alan modeli (başlangıç)

Varlıklar: `ApplicationUser` (+rol), `PatientProfile`, `Complaint`, `ComplaintMessage`, `Attachment`, `DoctorReview`, `AppointmentRequest`, `ConsentRecord`, `AuditEvent`.
Çekirdekte ayrıca: `DoctorProfile` (branş: KBB / Göğüs Hastalıkları, sentetik diploma/tescil no, aktif/pasif), `DoctorAvailability`, `Notification`.
AI aşamasında eklenecek: `AnalysisJob`, `AnalysisResult`.
Sonraki aşamalarda: `PrescriptionDraft`, `DoctorComment`, `SatisfactionSurvey`, `Visit` (görüntülü görüşme).

Kurallar:
- Hasta, başvuru, randevu ve görüşme **ayrı kavramlardır.** Tek tabloda birleştirme.
- `Complaint` bir `Revision` (tamsayı) taşır. Hasta metni değiştirirse yeni revizyon oluşur; eski kayıt silinmez.
- Zaman alanları: `CreatedAt` (sistem kayıt zamanı, UTC) ile `ReportedOnset` (hastanın beyan ettiği başlangıç) ayrı tutulur.
- Bilgi kaynağı türü kaydedilir: `PatientReport`, `DoctorReview`, `ImportedRecord`. Hasta beyanı doğrulanmış klinik bilgi değildir.
- **"Bilinmiyor" ile "yok" ayrıdır.** Boş liste ikisini temsil etmez. Örnek: `AllergyStatus = Unknown | ReportedNone | VerifiedNone`; ilaç, ateş vb. için de aynı yaklaşım.
- Aile öyküsü (ör. "annemde diyabet var") hastanın kendi hastalığı olarak kaydedilmez; ayrı alan (`FamilyHistory`).
- Doktor düzeltmesi orijinal AI çıktısını **silmez.** İlk çıktı, düzeltilmiş çıktı, değiştiren kişi ve zaman saklanır.
- Dosya içeriği veritabanına büyük metin olarak konmaz; dosya deposu + metadata (tür, boyut, hash, yükleyen, saklama anahtarı, zaman).
- Kayıt silme: sağlık kayıtlarında fiziksel silme yerine durum/“soft delete” + saklama politikası. `[BELİRLENECEK]`

## 7. Durum makineleri

- Başvuru: `Draft → Submitted → UnderReview → Reviewed → Closed`.
- AI işi (sonra): `Pending → Running → Completed | Failed | Cancelled`.
- Başvuru durumu ile AI işi durumu **ayrı** tutulur. AI başarısız olsa da doktor orijinal metinle devam edebilir.
- Randevu: `Requested → Proposed → Confirmed | Rejected | Cancelled`. Bildirim durumu (`Pending | Sent | Failed`) iş kaydından ayrıdır; bildirim başarısızsa randevu/doktor yanıtı iptal sayılmaz.
- Her durum geçişi için **kim yapabilir** tanımlı olmalı ve **sunucuda** denetlenmelidir. Buton gizlemek yetki kontrolü değildir.
- Geçersiz geçiş denemesi hata döner ve audit'e yazılır.

## 8. Yetkilendirme ve güvenlik (zorunlu)

- Authentication ≠ authorization. `Doctor` rolü her hastaya erişim hakkı vermez: **kayıt bazlı yetkilendirme** (atanmış/izinli doktor) uygula (resource-based authorization).
- Hasta yalnızca kendi başvurusunu, dosyasını ve sonuçlarını görür. Yönetici teknik yönetim yapar; sağlık içeriğini okumak otomatik hakkı değildir.
- Dosya indirme, AI sonucu sorgulama, doktor notu güncelleme dahil **her uç noktada** oturum + rol + kayıt ilişkisi kontrol edilir. URL/kayıt kimliğini tahmin edilemez yapmak yetki kontrolünün yerine geçmez (IDOR'a karşı test yaz).
- Kayıt formundan yalnızca **Hasta** hesabı açılır. Doktor ve yönetici hesabı yalnızca yönetici tarafından oluşturulur/onaylanır; rol, istemciden gelen alanla atanamaz (mass assignment'a karşı test yaz).
- Pasif doktor yeni başvuru üstlenemez ve panele giremez.
- Tüm POST/PUT/DELETE formlarında CSRF koruması (anti-forgery token). Kullanıcı girdisi View'da kodlanarak gösterilir (XSS); `Html.Raw` yasak (gerekçeli istisna hariç).
- Parola politikası, hesap kilitleme, oturum zaman aşımı, güvenli çerez ayarları (HttpOnly, Secure, SameSite), HTTPS zorunlu, güvenlik başlıkları.
- Rate limiting: giriş, başvuru gönderme, dosya yükleme.
- Dosya yükleme: boyut ve tür beyaz listesi, içerik türü doğrulama, orijinal dosya adını depolama yolu olarak kullanma, metadata temizliği için not düş.
- Gizli bilgi (bağlantı dizesi, API anahtarı, parola) kaynak koda, `appsettings.json`'a veya commit'e **konmaz.** User Secrets / ortam değişkeni kullan. `.gitignore` bunları kapsar.
- Seed edilen kullanıcıların parolaları yalnızca geliştirme ortamında ve belgelenmiş sentetik hesaplardır.

### 8.1 Başvuru atama `[VARSAYILAN]`

- Gönderilen başvuru, hastanın seçtiği (veya sonra AI'ın önerdiği) **branş kuyruğuna** düşer.
- O branştaki aktif doktor başvuruyu **üstlenir** (claim). Üstlenme atomiktir: iki doktor aynı anda üstlenemez (concurrency testi).
- Kuyruktaki başvurunun yalnızca özet satırı (yaş aralığı, branş, öncelik etiketi, bekleme süresi) görünür; ayrıntıya yalnızca üstlenen doktor erişir.
- Yeniden atama yalnızca yönetici yapar ve audit'e yazılır.

## 9. Gizlilik, KVKK ve yasal uyum

Bu proje bir prototiptir; **"KVKK/mevzuata uyumludur" ifadesi kullanılmaz.** Uyum, uzman değerlendirmesi gerektirir. Teknik olarak şunlar baştan tasarlanır:

- **Sadece sentetik veri.** Gerçek hasta verisi geliştirme, test, demo veya yapay zekâ aracına yapıştırma amaçlı kullanılmaz.
- `ConsentRecord`: `UserId`, `Purpose`, `TextVersion`, `Response`, `Timestamp`, `WithdrawnAt`. Amaçlar **ayrı** kayıtlardır (örn. başvuru işleme, ses işleme, ses saklama, eğitim amaçlı yeniden kullanım, bildirim). Tek bir "her şeyi kabul ediyorum" kutusu yasaktır.
- İzin metni sürümlüdür; kullanıcının hangi sürümü kabul ettiği kaydedilir. İzin geri çekme akışı vardır; geri çekilince hangi işlemlerin duracağı tanımlıdır.
- Veri minimizasyonu: bir servise/modele yalnızca görev için gereken veri gönderilir. T.C. kimlik no, adres, telefon özetleme/analiz servisine gönderilmez.
- `AuditEvent`: kim, hangi kayda, ne zaman, hangi işlemi yaptı (okuma dahil). Audit kaydı hassas olabilir; erişimi kısıtlı.
- Teknik log ≠ audit log. **Şikâyet metninin tamamı otomatik loglanmaz.** Loglarda kimlik/sağlık içeriği maskelenir; `CorrelationId` kullan.
- Saklama süresi ve silme/erişim talebi süreçleri için yer ayır `[BELİRLENECEK]`.
- Kullanıcıya görünür açıklama: "Bu sistem tanı koymaz; bir doktorun yerine geçmez." Acil durum uyarısı (112) başvuru formunda sabit gösterilir.
- Reçete, e-Nabız, e-Devlet gibi resmî entegrasyonlarda hukuki ve kurumsal şartlar ayrıca araştırılır; kod yalnızca adapter + demo sağlar.

## 10. Klinik güvenlik kuralları (pazarlık konusu değil)

- Sistem **tanı koymaz**, **tedavi veya ilaç önermez**, doz bilgisi vermez.
- İzin verilen çıktılar: şikâyetin kaynağa bağlı özeti, eksik bilgi soruları, uyarı/bilgilendirme (uzman onaylı), branş yönlendirme önerisi (KBB / göğüs hastalıkları vb.), doktor incelemesi önerisi.
- **Klinik eşik, risk puanı veya aciliyet kuralını kendin uydurma.** Ajan klinik kural üretmez. Gerekirse sentetik işaret (`ExpertApprovedWarning = true`) ile akışı göster ve kodda "DEMO KURALI — klinik geçerliliği yoktur" notu bırak. Gerçek kural, uzman onayı + sürüm + test senaryosu ile eklenir.
- Hiçbir kural eşleşmezse sonuç "düşük risk" olmaz; **"değerlendirilemedi / inceleme gerekli"** olur.
- "Randevuya zorlama" yalnızca uzman onaylı kural eşleştiğinde ve kademeli çalışır (bilgilendirme → doktor incelemesi önerisi → öncelikli inceleme kuyruğu). Gerçek acil için "112" yönlendirmesi gösterilir.
- AI/model çıktısındaki `approved: true` gibi alanlar **doktor onayı sayılmaz.** Doktor onayı gerçek oturum kullanıcısı ve zaman damgasıyla sunucuda kaydedilir.
- Hasta tarafından bildirilen ifade (ör. hastanın yazdığı tanı) sistem tarafından doğrulanmış teşhis gibi tekrarlanmaz.
- Güven skoru (`confidence`) kalibre edilmedikçe "yüzde güven" olarak gösterilmez.
- **Bütün şikâyetler aktarılır:** özet, hastanın bildirdiği hiçbir şikâyeti sessizce düşürmez; tekrarlar birleştirilebilir. Doktor ekranında orijinal metin her zaman özetin yanında görünür.
- **Sıralama ("büyükten küçüğe"):** yalnızca sürümlü, uzman onaylı kural tablosuyla ve her satırda gerekçesiyle yapılır. Kural yoksa sıralama, hastanın yazdığı sırayla gösterilir ve "öncelik sıralaması yapılmadı" yazılır.
- **Branş/doktor önerisi:** izin verilir (KBB, Göğüs Hastalıkları, "uygun branş belirlenemedi"). Doktor önerisi branş ve müsaitliğe dayanır; puan/reklam sıralaması yapılmaz.
- **Reçete:** sistem ilaç önermez ve reçete içeriği üretmez. Doktor reçete taslağını kendisi yazar; gönderim yalnızca oturumdaki doktorun açık onayıyla yapılır ve audit'e yazılır. Şimdilik `StubPrescriptionGateway` "DEMO — gerçek reçete gönderilmedi" döner.
- **Ses analizi:** ASR (ses→metin) ile akustik hastalık tahmini ayrı işlerdir. Akustik tahmin araştırma hedefidir; hastaya sonuç olarak gösterilmez.

## 11. API ve dayanıklılık kuralları

- Dış çağrılar typed `HttpClient` / `IHttpClientFactory` ile; base address, timeout ve başlıklar yapılandırmada.
- Tekrar gönderimde çift kayıt olmasın: **idempotency anahtarı** (aynı anahtar + aynı içerik = tek kayıt; aynı anahtar + farklı içerik = çatışma hatası).
- İki doktor aynı kaydı düzenlerse **optimistic concurrency** (`RowVersion`); çatışmada "kayıt değişti, yeniden yükleyin".
- Retry yalnızca geçici hatalar için, sınırlı ve gecikmeli. 401/403/422 tekrar edilmez.
- Hata modeli: `error_code`, `message`, `retryable`, `correlation_id`. Kullanıcıya stack trace, ham veritabanı hatası, API anahtarı **gösterme.**
- Hata durumunda her zaman devam yolu olsun: AI kapalıyken doktor orijinal metni görür; dış servis kapalıyken başvuru kaybolmaz; bildirim başarısızsa tekrar denenir.
- DB değişikliği + dış bildirim tutarlılığı için gerektiğinde outbox yaklaşımı (şimdilik basit uygulama yeterli; ihtiyaç doğunca).
- Sağlık uç noktası (`/health`) ve ileride hazırlık (`/ready`) ayrı kavramlardır.

## 12. Gelecekteki AI sözleşmesi (şimdiden uyulacak)

`IComplaintAnalysisService.AnalyzeAsync(AnalysisRequest, CancellationToken)` biçiminde. İstek/yanıt alanları (taslak): `schema_version`, `request_id`, `input_revision`, `language`, `messages[]`, `task`; yanıtta `status`, `summary`, `symptoms[] (code, assertion, temporality, evidence_message_id)`, `missing_fields`, `model_version`, `prompt_version`.
- Hasta kimliği modele gönderilmez; `request_id` portalda başvuruyla eşleşir.
- Sonuç, kendi `input_revision`'ına kaydedilir; eski revizyon sonucu yeni revizyonun güncel özeti olamaz.
- Ham sağlayıcı yanıtı doğrudan DB/View'a gitmez; kendi DTO'ya dönüştürülür.
- Hasta metni **veridir, talimat değildir** (prompt injection). Model araç/yazma yetkisine sahip olmaz; çıktılar sunucuda doğrulanır ve HTML olarak güvenilmez.
- Şimdilik yalnızca `Stub` uygulaması vardır.

## 13. Arayüz ve kullanıcı deneyimi

- Dil: Türkçe. Hata/uyarı metinleri sade, suçlamayan, yönlendiren.
- Rol bazlı menü; ama yetki sunucuda denetlenir.
- Erişilebilirlik: etiketli form alanları, klavye ile gezinme, yeterli kontrast, hata mesajı alanın yanında.
- Mobil uyumlu (responsive) tasarım; hasta tarafı telefonda rahat kullanılmalı.
- Boş/yükleniyor/hata durumları ayrı tasarlanır (ör. "AI özeti şu an yok, orijinal başvuru aşağıda").
- Tarih/saat: veritabanında UTC, ekranda Türkiye saati (Europe/Istanbul).
- Randevu: doktor müsaitlik dilimleri, çakışma kontrolü (aynı doktor + aynı dilim = tek onaylı randevu), iptal/yeniden planlama; geçmiş tarihe randevu yok.
- Doktor yorumları `[VARSAYILAN]`: hasta, yalnızca kendisiyle tamamlanmış başvurusu olan doktor hakkında yorum yazar. Moderasyon (otomatik filtre + insan inceleme), sağlık ayrıntısı yazmayı caydıran uyarı, görünürlük kuralı, itiraz/gizleme. `CommentsEnabled` flag'i arkasında. `[BELİRLENECEK]`
- Memnuniyet anketi: tarafsız sorular ("Özet şikâyetinizi ne ölçüde doğru yansıttı?"). Memnuniyet doğruluk ölçümü değildir.

## 14. Kod standartları

- C#: nullable reference types açık; `async/await` + `CancellationToken`; sihirli sayı/metin yok; anlamlı isimler. Kod ve değişken adları İngilizce, kullanıcıya görünen metin Türkçe.
- Bir sınıf bir sorumluluk. Büyük metotları böl.
- Yorumlar "ne"yi değil "neden"i anlatır. Demo/sahte davranış açıkça `// DEMO` ile işaretlenir.
- Hata yutma (`catch {}`) yasak. Özel istisna veya sonuç nesnesi kullan.
- Veri erişimi: EF Core; ham SQL gerekiyorsa parametreli. N+1 sorgulara dikkat.
- Her şema değişikliği EF **migration** ile; migration adı anlamlı.
- Ölü kod, yorum satırına alınmış kod ve kullanılmayan paket bırakma.

## 15. Test kuralları

Her özellik için en az **bir olumlu ve bir başarısızlık** senaryosu. Senaryo biçimi: Given / When / Then.
Öncelikli testler:
1. Normal başvuru gönderimi; boş metin reddi.
2. Çift gönderimde (aynı idempotency anahtarı) tek başvuru.
3. Eski revizyon sonucu yeni revizyonu ezmez.
4. Yetkisiz doktor / başka hasta erişimi reddedilir (403/404).
5. Geçersiz durum geçişi reddedilir.
6. Bilinmiyor/yok ayrımı korunur; aile öyküsü kişiye karışmaz.
7. AI/dış servis kapalıyken akış devam eder.
8. Bildirim başarısızken doktor yanıtı korunur; tekrar deneme mükerrer yanıt üretmez.
9. İki doktorun eşzamanlı düzenlemesi yakalanır.
10. İzin geri çekilince ilgili işlem durur.

"Çalışıyor" dört seviyedir: birim, entegrasyon, uçtan uca, kurulum. Hangisini doğruladığını belirt.

## 16. Çalışma biçimi (ajan için)

1. **Önce plan, sonra kod.** Büyük görevlerde önce kısa plan sun, onay bekle.
2. **Küçük görev, küçük değişiklik.** Tek seferde bütün projeyi yazma. Her görev: kullanıcı ihtiyacı, girdi/çıktı, sorumlu modül, normal ve hata kabul kriteri, test.
3. Değişiklikten sonra: ne değişti, hangi dosyalar, nasıl test edilir, bilinen eksikler. Çalıştırmadığın şeyi "çalışıyor" diye sunma.
4. Emin olmadığın klinik, hukuki veya mimari karar için **tahmin etme, sor.**
5. Yeni bağımlılık, yeni tablo veya mimari sapma gerektiğinde önce gerekçe sun.
6. Bu dosyadaki bir kuralı değiştirmeyi gerektiren durumda değişikliği öner; kendi başına değiştirme.
7. Kullanıcı öğrenci ve projeyi savunacak kişidir: karmaşık kodun yanına kısa açıklama ver; "sihirli" çözüm yerine anlaşılır çözüm tercih et.
8. Git: küçük, anlamlı commit'ler (`feat:`, `fix:`, `test:`, `docs:`); `main` korumalı kabul edilir, özellik dalı kullan.

## 17. Tamamlandı (Definition of Done)

Bir görev bitmiş sayılır eğer: kullanıcı davranışı çalışıyor; normal ve hata senaryosu testlenmiş; yetki kontrolü sunucuda; migration/yapılandırma belgelenmiş; gizli bilgi sızmamış; sentetik veri kullanılmış; `README`/`docs` güncel; temiz bir ortamda kurulup çalıştırılabiliyor.

## 18. Yasaklar (kısa liste)

- Tanı, ilaç/doz, tedavi önerisi üretme.
- Klinik eşik/risk kuralı uydurma.
- Gerçek hasta verisi kullanma veya yapay zekâya yapıştırma.
- Yetkiyi yalnızca arayüzde (buton gizleme) uygulama.
- Gizli bilgiyi koda/commit'e koyma.
- Şikâyet metnini loglama.
- Stub'ı gerçek entegrasyon gibi sunma; "yasal olarak uyumlu" iddiası.
- Test etmeden "tamamlandı" deme.
- Kapsam dışı modülleri (NLP eğitimi, ses/görüntü modeli, gerçek e-Devlet) bu aşamada gerçekleştirmeye çalışma.

## 19. Açık kararlar (hocayla netleştirilecek)

- Hoca notlarındaki boş madde neydi?
- "Randevuya zorlama" tanımı (öneri mi, kuyrukta öne alma mı).
- Atama modeli (§8.1 varsayılanı kabul mü?).
- 18 yaş altı hasta / vekil hesabı kapsamda mı?
- Görüntülü görüşme bu dönem gerekli mi?
- Klinik kuralları onaylayacak uzman var mı?
- Gerçek e-Nabız/e-Devlet test erişimi var mı, yoksa adapter + demo mu kabul?
- Veritabanı seçimi, saklama süreleri, yorum modülünün görünürlük kuralları.
- Rapor ve ara teslim tarihleri.
