# Araç Kiralama Sistemi — Proje Raporu

## Özet

Bu doküman, "Araç Kiralama Sistemi" adlı ders projesinin (BLM2058 Veritabanı Yönetimi) teknik raporudur. Proje ASP.NET Core 9 Razor Pages, Entity Framework Core ve PostgreSQL kullanılarak geliştirilmiştir. Proje altyapısı tamamlanmış olup ilerleme %60 seviyesindedir; temel servis katmanı, veri modeli ve UI iskeleti mevcuttur. Kalan iş: 14 Razor sayfası, containerizasyon ve test/polish aşamaları.

## Amaç

Bu projenin amacı, araç kiralama süreçlerini yönetebilen; müşteri, araç, kira, ödeme ve hasar raporu kayıtlarını tutan; iş kurallarını hem uygulama katmanında hem de veritabanı seviyesinde enforce eden örnek bir web uygulaması sunmaktır.

## Kapsam

- Müşteri yönetimi (CRUD)
- Araç yönetimi (katalog, durum takibi)
- Kira yönetimi (pending → active → completed)
- Ödeme kaydı (rental ile 1:1 ilişki)
- Hasar raporları (sadece tamamlanmış kiralamalar için)

## Teknoloji Yığını

- Framework: ASP.NET Core 9 (Razor Pages)
- ORM: Entity Framework Core (Database-First)
- Veri Tabanı: PostgreSQL (10 tablo + trigger)
- UI: Razor Pages + Bootstrap 5
- Dil: C# 12

## Mimari ve Katmanlar

Uygulama katmanları şu şekilde yapılandırılmıştır:

- UI (Razor Pages) — `Pages/` içinde sayfa modelleri ve .cshtml dosyaları
- Servis Katmanı — `Services/` altında iş kuralları ve veri erişim kapsülleri
- Veri Katmanı — EF Core `CarRentalContext` (Database-First)
- PostgreSQL — tablo, constraint ve trigger tanımları (`database_schema.sql`)

Servisler dependency injection ile `Program.cs` içinde kaydedilmiş ve PageModel'ler aracılığıyla kullanılmıştır.

## Veritabanı Özeti

Veritabanında 10 temel tablo bulunmaktadır: `CUSTOMER`, `VEHICLE_CATEGORY`, `BRANCH`, `STAFF`, `VEHICLE`, `FEATURE`, `VEHICLE_FEATURE`, `RENTAL`, `PAYMENT`, `DAMAGE_REPORT`.

Öne çıkan özellikler:

- Yabancı anahtarlar ve referential integrity
- CHECK ve UNIQUE kısıtları (ör. araç durumları, ödeme yöntemleri)
- `trg_rental_completed` tetikleyicisi: kiralama tamamlandığında aracın durumunu ve konumunu otomatik günceller
- Sample veri: `sample_data.sql` içinde ~60 kayıt

## Uygulama - Teknik Detaylar

- Model validasyonları: `[Required]`, `[StringLength]`, `[Range]`, `[Display]` gibi attribute'lar kullanılmıştır.
- Servisler: `IGenericService<T>` üzerine inşa edilmiş `GenericService<T>` ve özel servisler (CustomerService, RentalService, vb.).
- İş kuralları (örnekler):
  - KR-01: Aynı araç için çakışan kiralama engellenir (RentalService.HasOverlappingRentalAsync)
  - KR-02: Plaka/lisans numarası benzersiz olmalıdır
  - KR-03: Hasar raporu sadece tamamlanmış kiralamalara eklenebilir
  - KR-04: Bir kiralama için en fazla 1 ödeme (DB UNIQUE + servis kontrol)

## Kurulum ve Çalıştırma

Özet kurulum adımları (ayrıntılar `README.md` içinde):

1. PostgreSQL kur ve başlat
2. `database_schema.sql` ile veritabanını oluştur
3. `sample_data.sql` ile örnek veriyi yükle (isteğe bağlı)
4. `CarRental/appsettings.json` içindeki connection string'i güncelle
5. `dotnet build` ve `dotnet run` (kök dizin: `CarRental/`)

Hızlı komutlar:

```
cd CarRental
dotnet build
dotnet run
```

Uygulama varsayılan olarak `https://localhost:5001/` adresinde çalışır.

## Mevcut Durum (Özet)

- Servis Katmanı: Tamamlandı (8 servis, 50+ metot)
- Modeller & DbContext: Tamamlandı (Database-First, 9 model)
- UI İskeleti: `Pages/Shared/_Layout.cshtml` ve `Index` (dashboard) tamam
- Razor Pages: 1/17 tamamlandı (dashboard) — 14 sayfa eksik
- Dokümantasyon: `README.md`, `PROJECT_SUMMARY.md`, `RAZOR_PAGES_GUIDE.md` mevcut

## Kalan İşler ve Önerilen Yol Haritası

Kısa vadede (3-4 saat):

1. `Customers/Index, Create, Edit, Details` sayfalarını uygula (Pattern 1 & 2)
2. `Rentals/Create` (çakışma validasyonu) ve `Rentals/Index` sayfalarını uygula
3. `Rentals/Activate` ve `Rentals/Complete` işlemlerini test et (tetikleyici doğrulaması)

Orta vadede:

- Payments ve DamageReports sayfaları
- Filtreleme, sıralama ve sayfalama ekleme
- Dockerfile + docker-compose ile containerization

## Örnek SQL Sorguları ve Açıklama

Not: Bu bölümdeki sorgular artık uygulama arayüzünden de çalıştırılabilmektedir. Uygulamada `Daha Fazla > Raporlar / SQL Sorguları` menüsü üzerinden ilgili sorgu seçilip sonuçlar tablo olarak görüntülenebilir.

### Sorgu 1: Müsait Araçları Listeleme

Amaç: Sistemde durumu `available` olan araçları, kategori ve şube bilgisiyle birlikte günlük kiralama ücretine göre sıralı göstermektir.

```sql
SELECT
  v.vehicle_id,
  v.plate_number,
  v.brand,
  v.model,
  v.year,
  v.daily_price,
  vc.name AS category,
  b.city AS branch
FROM VEHICLE v
JOIN VEHICLE_CATEGORY vc ON v.category_id = vc.category_id
JOIN BRANCH b ON v.branch_id = b.branch_id
WHERE v.status = 'available'
ORDER BY v.daily_price;
```

Rapor yorumu (eklenebilir):

- Bu sorgu operasyonel olarak müşteriye uygun araç önerimi için temel veri setini üretir.
- `JOIN` kullanımı sayesinde araçların sadece teknik bilgisi değil, iş açısından anlamlı olan kategori ve şube bilgisi de tek sorguda alınır.
- `ORDER BY v.daily_price` ifadesi fiyat odaklı listeleme için kullanıcı deneyimini iyileştirir.

Ekran görüntüsü yerleşimi:

- Bu başlığın hemen altına "Şekil 1. Müsait Araçları Listeleme Sorgusu" olarak SQL çıktısının ekran görüntüsü eklenmelidir.
- Görsel alt yazıda sorgunun amacı tek cümle ile belirtilmelidir.

## Test Planı (Kısa)

- Birim testi altyapısı henüz eklenmedi; manuel test adımları:
  - Örnek veri yüklü iken dashboard ve listeler kontrol edilecek
  - Kiralama oluşturma: uygun olmayan durumlarda servis hata vermeli
  - Kiralama tamamlandığında `VEHICLE` tablosu tetikleyiciyle güncellenmeli
  - Ödeme ekleme: aynı kiralama için ikinci ödeme engellenmeli

## Sonuç ve Özet Mesaj

Bu proje, gerçek dünya iş kurallarını hem veritabanı hem de uygulama katmanında yöneten, eğitim amaçlı güçlü bir örnektir. Mevcut durumda altyapı ve servisler kullanılmaya hazır; eksik kalan Razor Pages implementasyonları tamamlandığında proje üretime hazır bir demo hâline gelecektir.

---

## Ekler

- `database_schema.sql` — Veritabanı yapısı ve trigger
- `sample_data.sql` — Test verileri
- `RAZOR_PAGES_GUIDE.md` — Sayfa kalıpları ve şablonlar
- `CarRental/` — Kaynak kodu ve `Program.cs`, `appsettings.json`
