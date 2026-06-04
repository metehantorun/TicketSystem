# Teknik Destek / Ticket Sistemi
### ASP.NET Core MVC - Ödev Projesi

---

## Proje Hakkında

Bu proje, müşterilerin destek talebi oluşturabildiği, destek ekibinin talepleri yönettiği tam işlevli bir **Teknik Destek (Ticket) Sistemi**dir.

---

## Karşılanan Gereksinimler

### ✅ 1. Veritabanı
- **EF Core Code-First** yaklaşımı kullanılmıştır.
- `AppDbContext` ile `DbContext` yapılandırması tamamlanmıştır.
- `dotnet ef migrations add` ile Migration oluşturulur.

### ✅ 2. Authentication & Authorization
- **ASP.NET Core Identity** kullanılmıştır.
- Kullanıcı girişi, kayıt ve çıkış işlemleri `AccountController` ile yönetilir.
- **Role-based yetkilendirme** uygulanmıştır:
  - **Admin** (Destek Ekibi): Tüm talepleri görür, durum günceller, talebi üstlenir.
  - **User** (Müşteri): Yalnızca kendi taleplerini oluşturur ve görüntüler.

### ✅ 3. UI & Layout
- `_Layout.cshtml` ile ortak **Header/Footer** yapısı oluşturulmuştur.
- **ViewModel** kullanımı: `CreateTicketViewModel`, `TicketDetailViewModel`, `TicketListViewModel`, `AddReplyViewModel`, `LoginViewModel`, `RegisterViewModel`
- ViewBag de bazı basit veri taşıma amaçlı kullanılmıştır.

### ✅ 4. Validations
- **Data Annotations** ile form validasyonu uygulanmıştır:
  - `[Required]`, `[StringLength]`, `[EmailAddress]`, `[DataType]`, `[Compare]`
- Client-side ve server-side validasyon aktif.

### ✅ 5. Enum Kullanımı
- `TicketStatus`: `Acik`, `Cozuldu`, `Kapandi`
- `TicketPriority`: `Dusuk`, `Orta`, `Yuksek`, `Kritik`

### ✅ 6. LINQ ile Filtreleme
- Sadece açık talepleri getirme:
  ```csharp
  query.Where(t => t.Status == TicketStatus.Acik)
  ```
- Müşteriye göre filtreleme, önceliğe göre filtreleme, arama gibi LINQ sorguları uygulanmıştır.

### ✅ 7. Müşteri Özellikleri
- Destek talebi oluşturma
- Talebin durumunu (Açık, Çözüldü, Kapandı) izleme
- Taleplerim sayfasında tüm talepleri ve özellikle açık talepleri görme

### ✅ 8. Destek Ekibi Özellikleri
- Talepleri üstlenme (Assign)
- Cevap yazma
- Durum güncelleme (Açık / Çözüldü / Kapandı)

---

## Proje Yapısı

```
TicketSystem/
│
├── Controllers/
│   ├── AccountController.cs   → Login, Register, Logout
│   ├── HomeController.cs      → Dashboard (istatistikler)
│   └── TicketController.cs    → CRUD, Assign, UpdateStatus, AddReply
│
├── Models/
│   ├── Enums.cs               → TicketStatus, TicketPriority
│   ├── ApplicationUser.cs     → Identity User
│   ├── Ticket.cs              → Talep modeli + Data Annotations
│   ├── TicketReply.cs         → Cevap modeli + Data Annotations
│   └── ViewModels/
│       └── ViewModels.cs      → Tüm ViewModel'lar
│
├── Data/
│   ├── AppDbContext.cs        → EF Core DbContext
│   └── DbSeeder.cs            → Rol ve kullanıcı seed
│
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml     → Ortak Header/Footer
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Account/               → Login, Register, AccessDenied
│   ├── Home/                  → Dashboard
│   └── Ticket/                → Index, Create, Detail, MyTickets
│
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
│
├── appsettings.json           → Connection String
├── Program.cs                 → DI, Middleware, Seed
└── TicketSystem.csproj
```

---

## Kurulum ve Çalıştırma

### Ön Koşullar
- .NET 8 SDK
- SQL Server (LocalDB veya Express)
- Visual Studio 2022 veya VS Code

### Adımlar

```bash
# 1. Projeyi aç
cd TicketSystem

# 2. Paketleri yükle
dotnet restore

# 3. Migration oluştur (ilk sefer)
dotnet ef migrations add InitialCreate

# 4. Veritabanını güncelle
dotnet ef database update

# 5. Projeyi çalıştır
dotnet run
```

> **Not:** `Program.cs` içindeki `DbSeeder` otomatik olarak veritabanını oluşturur ve seed eder.

### Varsayılan Giriş Bilgileri

| Rol | Email | Şifre |
|-----|-------|-------|
| **Admin** (Destek Ekibi) | admin@ticketsystem.com | Admin123! |
| **User** (Müşteri) | musteri@ticketsystem.com | User123! |

---

## Ekranlar

| Ekran | Açıklama |
|-------|----------|
| Login | Kullanıcı girişi |
| Register | Yeni müşteri kaydı |
| Dashboard | İstatistik kartları |
| Ticket Listesi | Filtrelenebilir talep tablosu |
| Yeni Talep | Form validasyonlu talep oluşturma |
| Talep Detayı | Cevaplar, durum güncelleme, atama |
| Taleplerim | Müşterinin kendi talepleri |

---

## Teknolojiler

- ASP.NET Core MVC 8
- Entity Framework Core 8 (Code-First)
- ASP.NET Core Identity
- Bootstrap 5.3
- Bootstrap Icons
- jQuery Validation (unobtrusive)
- SQL Server (LocalDB)
