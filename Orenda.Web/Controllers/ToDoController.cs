using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using Orenda.Web.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orenda.Web.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        private readonly OrendaDbContext _context;
        private readonly ILogService _logService;

        public ToDoController(OrendaDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            IQueryable<ToDo> gorevSorgusu = _context.ToDos
                .Include(t => t.AtananKisi)
                .Include(t => t.Adimlar);

            if (!isAdmin)
            {
                // Eğer Admin değilse sadece kendine atananları görsün
                gorevSorgusu = gorevSorgusu.Where(t => t.AtananCalisanID == currentUserId);
            }

            var gorevler = await gorevSorgusu.OrderByDescending(g => g.BaslangicTarihi).ToListAsync();
            
            // Görev Atama Modal'ı için çalışan listesini gönder
            if (isAdmin)
            {
                ViewBag.Calisanlar = new SelectList(await _context.Kullanicilar.OrderBy(k => k.Ad).ToListAsync(), "CalisanID", "Ad");
            }
            else
            {
                // Admin değilse sadece kendisini seçebilsin
                var kendisi = await _context.Kullanicilar.Where(k => k.CalisanID == currentUserId).ToListAsync();
                ViewBag.Calisanlar = new SelectList(kendisi, "CalisanID", "Ad");
            }

            return View(gorevler);
        }

        // --- ADMIN SADECE ---

        [HttpPost]
        public async Task<IActionResult> Create(ToDo model)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

                model.AtayanCalisanID = currentUserId;
                if (model.BaslangicTarihi == null) model.BaslangicTarihi = DateTime.Now;

                _context.ToDos.Add(model);
                await _context.SaveChangesAsync();
                
                await _logService.LogAsync(currentUserId, "Görev Oluşturuldu", $"'{model.Baslik}' adında yeni bir görev oluşturuldu ve ilgili personele atandı.");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev != null)
            {
                _context.ToDos.Remove(gorev);
                await _context.SaveChangesAsync();

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
                await _logService.LogAsync(currentUserId, "Görev Silindi", $"'{gorev.Baslik}' adlı görev sistemden silindi.");
            }
            return RedirectToAction(nameof(Index));
        }

        // --- HERKES (DURUM GÜNCELLEME) ---

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string yeniDurum)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            // Kendi görevi mi veya admin mi kontrolü
            if (!isAdmin && gorev.AtananCalisanID != currentUserId)
            {
                return Unauthorized();
            }

            string eskiDurum = gorev.Durum;
            gorev.Durum = yeniDurum;

            // Eğer Tamamlandı ise bitiş tarihini atayabiliriz
            if (yeniDurum == "Tamamlandı" || yeniDurum == "Bitti")
            {
                gorev.BitisTarihi = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            
            await _logService.LogAsync(currentUserId, "Görev Durumu Değişti", $"'{gorev.Baslik}' görevinin durumu '{eskiDurum}' -> '{yeniDurum}' olarak güncellendi.");

            return RedirectToAction(nameof(Index));
        }
        
        // --- İŞ ADIMI (GÖREV BÖLÜMÜ) YÖNETİMİ ---

        [HttpPost]
        public async Task<IActionResult> AddSubTask(int gorevNo, string adimBaslik, int? agirlikYuzdesi)
        {
            var gorev = await _context.ToDos.FindAsync(gorevNo);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            // Kendi görevi mi veya admin mi kontrolü
            if (!isAdmin && gorev.AtananCalisanID != currentUserId)
            {
                return Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(adimBaslik))
            {
                var adim = new GorevAdimi
                {
                    GorevNo = gorevNo,
                    Baslik = adimBaslik,
                    AgirlikYuzdesi = agirlikYuzdesi,
                    TamamlandiMi = false
                };
                
                _context.GorevAdimlari.Add(adim);
                await _context.SaveChangesAsync();
                
                await _logService.LogAsync(currentUserId, "Görev Adımı Eklendi", $"'{gorev.Baslik}' görevine '{adimBaslik}' alt adımı eklendi.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSubTask(int adimId, bool completed)
        {
            var adim = await _context.GorevAdimlari.Include(a => a.ToDoTutucu).FirstOrDefaultAsync(a => a.AdimID == adimId);
            if (adim == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            if (!isAdmin && adim.ToDoTutucu.AtananCalisanID != currentUserId)
            {
                return Unauthorized();
            }

            adim.TamamlandiMi = completed;
            await _context.SaveChangesAsync();

            string durumStr = completed ? "Tamamlandı" : "Bekliyor";
            await _logService.LogAsync(currentUserId, "Görev Adımı Güncellendi", $"'{adim.ToDoTutucu.Baslik}' görevindeki '{adim.Baslik}' adımı {durumStr} olarak işaretlendi.");

            return RedirectToAction(nameof(Index));
        }

        // --- PLANLAMA VE ONAY AKIŞI ---
        
        [HttpPost]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            
            // Sadece göreve atanan kişi veya admin onaya gönderebilir
            if (!User.IsInRole("Admin") && gorev.AtananCalisanID != currentUserId)
            {
                return Unauthorized();
            }

            gorev.OnayDurumu = "Onay Bekliyor";
            await _context.SaveChangesAsync();

            await _logService.LogAsync(currentUserId, "Plan Onaya Gönderildi", $"'{gorev.Baslik}' kodlu görev planlaması onayına sunuldu.");

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> ReviewPlan(int id, string karar, string? onayNotu)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            
            // Sadece Admin veya Görevi Oluşturan (AtayanCalisanID) yönetici onaylayabilir
            // Eğer AtayanCalisanID null ise, sadece Admin'ler.
            bool isManager = User.IsInRole("Admin") || (gorev.AtayanCalisanID == currentUserId);

            if (!isManager)
            {
                return Unauthorized();
            }

            gorev.OnayDurumu = karar == "Onayla" ? "Onaylandı" : "Reddedildi";
            gorev.OnayNotu = onayNotu;
            
            await _context.SaveChangesAsync();

            string logMessage = karar == "Onayla" 
                ? $"'{gorev.Baslik}' görev planlaması onaylandı."
                : $"'{gorev.Baslik}' görev planı reddedildi. Not: {onayNotu}";

            await _logService.LogAsync(currentUserId, "Plan Değerlendirildi", logMessage);

            return RedirectToAction(nameof(Index));
        }
    }
}