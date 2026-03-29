using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;

namespace Orenda.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class WearOsController : ControllerBase
    {
        private readonly OrendaDbContext _context;

        public WearOsController(OrendaDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. Sağlık Verisi Gönderme Endpoint'i (POST)
        // ==========================================
        [HttpPost("health")]
        public async Task<IActionResult> PostHealthData([FromBody] HealthDataDto data)
        {
            if (data == null || string.IsNullOrEmpty(data.DeviceID))
            {
                return BadRequest(new { Message = "Geçersiz veri gönderildi." });
            }

            var calisan = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.RfidID == data.DeviceID);
            if (calisan == null)
            {
                return NotFound(new { Message = "Çalışan bulunamadı." });
            }

            var saglikVerisi = new SaglikVerisi
            {
                CalisanID = calisan.CalisanID,
                Nabiz = data.Nabiz,
                AdimSayisi = data.AdimSayisi,
                TarihSaat = data.TarihSaat ?? DateTime.Now
            };

            _context.SaglikVerileri.Add(saglikVerisi);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Sağlık verisi başarıyla kaydedildi.", Data = saglikVerisi });
        }

        // ==========================================
        // 2. Görevleri Çekme Endpoint'i (GET)
        // ==========================================
        [HttpGet("tasks/{deviceId}")]
        public async Task<IActionResult> GetTasks(string deviceId)
        {
            var calisan = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.RfidID == deviceId);
            if (calisan == null)
            {
                return Ok(new { Message = "Kullanıcı bulunamadı.", Tasks = new List<object>() });
            }

            var tasks = await _context.ToDos
                .Where(t => t.AtananCalisanID == calisan.CalisanID && t.Durum != "Tamamlandı")
                .Select(t => new
                {
                    TaskID = t.GorevNo,
                    Baslik = t.Baslik,
                    Aciklama = t.Aciklama,
                    Durum = t.Durum,
                    BitisTarihi = t.BitisTarihi
                })
                .OrderBy(t => t.BitisTarihi)
                .ToListAsync();

            if (!tasks.Any())
            {
                return Ok(new { Message = "Aktif görev bulunmuyor.", Tasks = new List<object>() });
            }

            return Ok(new { Message = "Görevler başarıyla getirildi.", Tasks = tasks });
        }

        // ==========================================
        // 3. Saat Üzerinden Kimlik Doğrulama (POST)
        // ==========================================
        [HttpPost("auth")]
        public async Task<IActionResult> AuthenticateWatch([FromBody] WatchAuthDto authData)
        {
            // Bu kısım ileride saatin ürettiği özel token veya pin ile çalışacak.
            // Şimdilik sadece çalışan var mı diye kontrol edelim.
            var calisan = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.CalisanID == authData.CalisanID);

            if (calisan == null)
            {
                return Unauthorized(new { Message = "Yetkisiz erişim veya böyle bir çalışan yok." });
            }

            // Kapı girişi veya saati sisteme kaydetme izni verildiği senaryosu.
            return Ok(new 
            { 
                Message = "Saat başarıyla doğrulandı.", 
                User = new { calisan.Ad, calisan.Soyad, calisan.AktiflikDurumu }
            });
        }
    }

    // API'nin beklediği JSON yapılarını temsil eden basit sınıflar (DTOs)
    public class HealthDataDto
    {
        public string DeviceID { get; set; } = string.Empty;
        public int Nabiz { get; set; }
        public int AdimSayisi { get; set; }
        public DateTime? TarihSaat { get; set; }
    }

    public class WatchAuthDto
    {
        public int CalisanID { get; set; }
        public string? PinCode { get; set; }
    }
}
