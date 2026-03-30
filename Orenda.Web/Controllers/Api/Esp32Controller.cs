using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using Orenda.Web.Services;
using System.Threading.Tasks;

namespace Orenda.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class Esp32Controller : ControllerBase
    {
        private readonly OrendaDbContext _context;
        private readonly ILogService _logService;

        // Basit bir memory-cache yapısı. Admin bir kullanıcı seçtiğinde bu ID'ye atanır.
        private static int? PendingRegistrationEmployeeId = null;

        public Esp32Controller(OrendaDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // ==========================================
        // 1. ESP32'nin Kart/Saat Okuttuğu Uç (HTTP POST)
        // ==========================================
        [HttpPost("scan")]
        public async Task<IActionResult> ScanCard([FromBody] Esp32ScanDto request)
        {
            if (string.IsNullOrEmpty(request.Uid))
            {
                return BadRequest(new { status = "HATA" });
            }

            var rfid = request.Uid.ToUpper().Trim();

            // SENARYO A: Kayıt Bekleniyorsa (Admin düğmeye basmışsa)
            if (PendingRegistrationEmployeeId.HasValue)
            {
                var userToRegister = await _context.Kullanicilar.FindAsync(PendingRegistrationEmployeeId.Value);
                if (userToRegister != null)
                {
                    userToRegister.RfidID = rfid;
                    await _context.SaveChangesAsync();

                    // Kayıt tamamlandı, bekleme modundan çık
                    PendingRegistrationEmployeeId = null;
                    return Ok(new { status = "KAYIT_BASARILI" });
                }
                
                // Kullanıcı bulunamazsa bekleme modundan çık
                PendingRegistrationEmployeeId = null;
            }

            // SENARYO B: Normal Kapı Okutma
            var kapiyiAcanKullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.RfidID == rfid);

            if (kapiyiAcanKullanici != null)
            {
                // Kullanıcı giriş yapmış kabul ediliyor
                kapiyiAcanKullanici.AktiflikDurumu = "Çevrimiçi";
                kapiyiAcanKullanici.SonGirisIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                
                await _context.SaveChangesAsync();
                
                // Sisteme giriş logu düş
                await _logService.LogAsync(kapiyiAcanKullanici.CalisanID, "Fiziksel Giriş Yapıldı", "ESP32/Saat okutarak sisteme giriş yapıldı (Kapı açıldı).");

                return Ok(new { status = "AC", ad = kapiyiAcanKullanici.Ad, soyad = kapiyiAcanKullanici.Soyad });
            }

            // Kimse bulunamadı
            return Ok(new { status = "RED" });
        }


        // ==========================================
        // 2. Admin Sayfasından Kaydı Başlatan Uç
        // ==========================================
        [HttpPost("start-registration/{calisanId}")]
        public IActionResult StartRegistration(int calisanId)
        {
            PendingRegistrationEmployeeId = calisanId;
            return Ok(new { message = "Sistem kayıt moduna alındı. Lütfen saati ESP32'ye okutun." });
        }

        // ==========================================
        // 3. Admin Kayıt Durumunu Kontrol Eden Uç
        // ==========================================
        [HttpGet("check-registration")]
        public IActionResult CheckRegistration()
        {
            // Eğer hala ID doluysa kayıt bekliyor, Null ise bitmiş (kart basılmış).
            if (PendingRegistrationEmployeeId.HasValue)
            {
                return Ok(new { isComplete = false });
            }
            return Ok(new { isComplete = true });
        }
    }

    public class Esp32ScanDto
    {
        public string Uid { get; set; } = string.Empty;
    }
}
