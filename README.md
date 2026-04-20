# Orenda - Akıllı Görev ve Ekip Yönetim Sistemi

Orenda, kurumsal ekiplerin iş akışlarını dijitalleştiren, akıllı saat (Wear OS) ve IoT (RFID) entegrasyonu ile personelin hem verimliliğini hem de sağlığını takip eden kapsamlı bir yönetim platformudur.

## 🚀 Öne Çıkan Özellikler

- **Gelişmiş Görev Yönetimi:** Görevlerin alt adımlara (Task Steps) bölünmesi, durum takibi ve hiyerarşik onay mekanizması.
- **Ekip ve Departman Yapısı:** Şirket içi hiyerarşiyi yöneten, rol bazlı erişim kontrolüne sahip yönetim paneli.
- **Wear OS Entegrasyonu:** Personelin akıllı saatleri üzerinden görev alması, durum güncellemesi yapması ve gerçek zamanlı sağlık verisi (Nabız vb.) transferi.
- **Sağlık Verisi Takibi:** Çalışanların zindelik durumlarını izleyen ve sisteme raporlayan özel modül.
- **Sistem Audit Log:** Tüm kullanıcı aktivitelerinin ve kritik sistem olaylarının takip edildiği gelişmiş günlükleme (Logging) sistemi.
- **Modern UI/UX:** Glassmorphism etkileri, akıcı animasyonlar ve tamamen responsive (mobil uyumlu) arayüz.

## 🛠️ Teknoloji Yığını

- **Backend:** .NET 8.0 ASP.NET Core MVC
- **Veritabanı:** MS SQL Server & Entity Framework Core (Code First)
- **Güvenlik:** Cookie-based Authentication & Role-based Authorization
- **Frontend:** HTML5, CSS3 (Vanilla), JavaScript (ES6+), FontAwesome
- **Entegrasyon:** RESTful API for Wear OS & Mobile devices

## 📂 Proje Yapısı

- `Orenda.Web`: Ana web uygulaması ve API katmanı.
- `Orenda.Models`: Veritabanı varlıkları ve veri modelleri.
- `Orenda.Data`: DbContext ve Migration dosyaları.
- `Orenda.Services`: İş mantığını (Business Logic) barındıran servis katmanı.

## 🔧 Kurulum

1. Depoyu yerel makinenize klonlayın:
   ```bash
   git clone https://github.com/KULLANICI_ADINIZ/Orenda.git
   ```
2. `Orenda.Web` klasöründeki `appsettings.Example.json` dosyasını `appsettings.json` olarak kopyalayın.
3. Kendi veritabanı bağlantı dizginizi (Connection String) `appsettings.json` dosyasına ekleyin.
4. Terminalden projeyi başlatın:
   ```bash
   dotnet restore
   dotnet run
   ```

## 🔒 Güvenlik Notu
Bu projede hassas veriler (veritabanı şifreleri vb.) `.gitignore` ile korunmaktadır. Kendi ortamınızda kurulum yaparken `appsettings.json` dosyasına gerçek verilerinizi güvenli bir şekilde eklemelisiniz.

---
*Bu proje, profesyonel bir portfolyo çalışması olarak geliştirilmiştir.*
