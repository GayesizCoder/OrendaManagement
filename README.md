**Orenda**, işletmelerin iç süreçlerini (personel, departman, görev, cihaz ve log yönetimi) dijitalleştiren kapsamlı bir yönetim platformudur. Bu proje, sadece bir web arayüzü sunmakla 
kalmaz; aynı zamanda dış donanımların (IoT cihazları ve akıllı saatler) sisteme veri gönderebilmesi için özel API uç noktaları (endpoints) barındırır.

Çalıştırmak için:
⚙️ Kurulum ve Çalıştırma

Projenin yerel bilgisayarınızda (Localhost) çalıştırılması için adımlar:

    Projeyi Klonlayın:
    Bash.
    1-git clone [https://github.com/GayesizCoder/OrendaManagement.git](https://github.com/GayesizCoder/OrendaManagement.git)

    Veritabanı Ayarlarını Yapın:
    appsettings.json dosyasını açın ve DefaultConnection kısmına kendi MS SQL Server bağlantı dizenizi (Connection String) girin.

    Veritabanını Oluşturun (Migrations):
    Package Manager Console (Visual Studio) üzerinden:
    Bash:
    1-Update-Database Veya .NET CLI üzerinden: dotnet ef database update

    Projeyi Başlatın:
    Bash: 
    1-dotnet run

## 🚀 Proje Mimarisi ve Öne Çıkan Özellikler

Bu proje, sürdürülebilir ve genişletilebilir olması adına kurumsal standartlara uygun olarak geliştirilmiştir:

* **MVC Mimarisi:** Model, View ve Controller katmanları birbirinden izole edilerek temiz kod (Clean Code) prensiplerine uyulmuştur.
* **Dependency Injection (Bağımlılık Enjeksiyonu):** `Services` katmanı (örn: `ILogService`) arayüzler (interfaces) üzerinden sisteme enjekte edilmiş, böylece kodun test edilebilirliği artırılmıştır.
* **Code-First Yaklaşımı:** Veritabanı (MS SQL) tabloları ve ilişkileri Entity Framework Core kullanılarak `Migrations` üzerinden C# kodlarıyla inşa edilmiştir.
* **IoT & Giyilebilir Teknoloji Entegrasyonu:** Web projesinin içine gömülü RESTful API'ler sayesinde ESP32 gibi mikrodenetleyiciler ve WearOS tabanlı akıllı saatler ile çift yönlü haberleşme sağlanmaktadır.

## 🛠️ Kullanılan Teknolojiler (Tech Stack)

* **Backend:** C#, ASP.NET Core 8.0 MVC
* **Veritabanı & ORM:** MS SQL Server, Entity Framework Core 8
* **Frontend:** Razor Pages (cshtml), Bootstrap, jQuery, HTML5/CSS3
* **Güvenlik & Loglama:** Özel Log Servisi (`LogController`, `SistemLog`), Authentication altyapısı.

## 📂 Klasör Yapısı (Folder Structure)

Proje ölçeklenebilir bir dizin yapısına sahiptir:

```text
Orenda.Web/
├── Controllers/
│   ├── Api/               # Cihaz haberleşmeleri için API uç noktaları
│   │   ├── Esp32Controller.cs
│   │   └── WearOsController.cs
│   ├── AccountController.cs # Kullanıcı giriş/çıkış işlemleri
│   ├── TeamController.cs    # Personel ve Departman yönetimi
│   └── ...
├── Models/                # Veritabanı tabloları (Kullanıcı, Departman, GorevAdimi vb.)
├── Views/                 # Razor HTML Şablonları ve Arayüzler
├── Services/              # İş mantığı ve arayüzler (ILogService, LogService)
├── Data/                  # EF Core DbContext ayarları (OrendaDbContext)
└── Migrations/            # Veritabanı versiyon kontrol dosyaları
