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

            var currentUser = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.CalisanID == currentUserId);
            int? userTakimId = currentUser?.TakimID;

            IQueryable<ToDo> gorevSorgusu = _context.ToDos
                .Include(t => t.AtananKisi)
                .Include(t => t.Takim)
                .Include(t => t.Adimlar);

            if (!isAdmin)
            {
                // EÄŸer Admin deÄŸilse kendine atananlarÄ± VEYA takÄ±mÄ±na atananlarÄ± gÃ¶rsÃ¼n
                gorevSorgusu = gorevSorgusu.Where(t => t.AtananCalisanID == currentUserId || (t.TakimID != null && t.TakimID == userTakimId));
            }

            var gorevler = await gorevSorgusu.OrderByDescending(g => g.BaslangicTarihi).ToListAsync();
            
            // GÃ¶rev Atama Modal'Ä± iÃ§in Ã§alÄ±ÅŸan ve takÄ±m listesini gÃ¶nder
            ViewBag.Takimlar = new SelectList(await _context.Takimlar.OrderBy(t => t.Ad).ToListAsync(), "TakimID", "Ad");
            if (isAdmin)
            {
                ViewBag.Calisanlar = new SelectList(await _context.Kullanicilar.OrderBy(k => k.Ad).ToListAsync(), "CalisanID", "Ad");
            }
            else
            {
                // Admin deÄŸilse sadece kendisini seÃ§ebilsin
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
            bool isAdmin = User.IsInRole("Admin");

                model.AtayanCalisanID = currentUserId;
                if (model.BaslangicTarihi == null) model.BaslangicTarihi = DateTime.Now;

                _context.ToDos.Add(model);
                await _context.SaveChangesAsync();
                
                await _logService.LogAsync(currentUserId, "GÃ¶rev OluÅŸturuldu", $"'{model.Baslik}' adÄ±nda yeni bir gÃ¶rev oluÅŸturuldu ve ilgili personele atandÄ±.");
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
            bool isAdmin = User.IsInRole("Admin");
                await _logService.LogAsync(currentUserId, "GÃ¶rev Silindi", $"'{gorev.Baslik}' adlÄ± gÃ¶rev sistemden silindi.");
            }
            return RedirectToAction(nameof(Index));
        }

        // --- HERKES (DURUM GÃœNCELLEME) ---

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string yeniDurum)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            // Kendi gÃ¶revi mi veya admin mi kontrolÃ¼
            bool isAssignee = gorev.AtananCalisanID == currentUserId;
            bool isTeamTask = false;
            if (gorev.TakimID.HasValue)
            {
                var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.CalisanID == currentUserId);
                isTeamTask = user?.TakimID == gorev.TakimID;
            }

            if (!isAdmin && !isAssignee && !isTeamTask)
            {
                return Unauthorized();
            }

            string eskiDurum = gorev.Durum;
            gorev.Durum = yeniDurum;

            // EÄŸer TamamlandÄ± ise bitiÅŸ tarihini atayabiliriz
            if (yeniDurum == "TamamlandÄ±" || yeniDurum == "Bitti")
            {
                gorev.BitisTarihi = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            
            await _logService.LogAsync(currentUserId, "Görev Durumu Değişti", $"'{gorev.Baslik}' görevinin durumu '{eskiDurum}' -> '{yeniDurum}' olarak güncellendi.");

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return await GetTaskCard(id);
            }

            return RedirectToAction(nameof(Index));
        }
        
        // --- Ä°Å ADIMI (GÃ–REV BÃ–LÃœMÃœ) YÃ–NETÄ°MÄ° ---

        [HttpPost]
        public async Task<IActionResult> AddSubTask(int gorevNo, string adimBaslik, int? agirlikYuzdesi)
        {
            var gorev = await _context.ToDos.FindAsync(gorevNo);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            // Kendi gÃ¶revi mi veya admin mi kontrolÃ¼
            bool isAssignee = gorev.AtananCalisanID == currentUserId;
            bool isTeamTask = false;
            if (gorev.TakimID.HasValue)
            {
                var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.CalisanID == currentUserId);
                isTeamTask = user?.TakimID == gorev.TakimID;
            }

            if (!isAdmin && !isAssignee && !isTeamTask)
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

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return await GetTaskCard(gorevNo);
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

            bool isAssignee = adim.ToDoTutucu.AtananCalisanID == currentUserId;
            bool isTeamTask = false;
            if (adim.ToDoTutucu.TakimID.HasValue)
            {
                var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.CalisanID == currentUserId);
                isTeamTask = user?.TakimID == adim.ToDoTutucu.TakimID;
            }

            if (!isAdmin && !isAssignee && !isTeamTask)
            {
                return Unauthorized();
            }

            adim.TamamlandiMi = completed;
            bool statusChanged = false;
            
            // Eğer %100 olduysa otomatik onaya gönder ve durumu Tamamlandı yap
            var masterTask = await _context.ToDos.Include(t=>t.Adimlar).FirstOrDefaultAsync(t=>t.GorevNo == adim.GorevNo);
            if (masterTask != null && masterTask.TamamlanmaOrani >= 100)
            {
                if (masterTask.OnayDurumu != "Onaylandı")
                {
                    masterTask.OnayDurumu = "Onay Bekliyor";
                }
                
                if (masterTask.Durum != "Tamamlandı")
                {
                    masterTask.Durum = "Tamamlandı";
                    masterTask.BitisTarihi = DateTime.Now;
                    statusChanged = true;
                }
            }

            await _context.SaveChangesAsync();

            string durumStr = completed ? "Tamamlandı" : "Bekliyor";
            await _logService.LogAsync(currentUserId, "Görev Adımı Güncellendi", $"'{adim.ToDoTutucu.Baslik}' görevindeki '{adim.Baslik}' adımı {durumStr} olarak işaretlendi.");

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (statusChanged)
                {
                    Response.Headers.Add("X-Status-Changed", "true");
                }
                return await GetTaskCard(adim.GorevNo);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> GetTaskCard(int id)
        {
            var gorev = await _context.ToDos
                .Include(t => t.AtananKisi)
                .Include(t => t.Takim)
                .Include(t => t.Adimlar)
                .FirstOrDefaultAsync(t => t.GorevNo == id);
            
            if (gorev == null) return NotFound();
            
            return PartialView("_TaskCardPartial", gorev);
        }

        // --- PLANLAMA VE ONAY AKIÅI ---
        
        [HttpPost]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");
            
            // Sadece gÃ¶reve atanan kiÅŸi, takÄ±m Ã¼yesi veya admin onaya gÃ¶nderebilir
            bool isAssignee = gorev.AtananCalisanID == currentUserId;
            bool isTeamTask = false;
            if (gorev.TakimID.HasValue)
            {
                var user = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.CalisanID == currentUserId);
                isTeamTask = user?.TakimID == gorev.TakimID;
            }

            if (!isAdmin && !isAssignee && !isTeamTask)
            {
                return Unauthorized();
            }

            gorev.OnayDurumu = "Onay Bekliyor";
            await _context.SaveChangesAsync();

            await _logService.LogAsync(currentUserId, "Plan Onaya GÃ¶nderildi", $"'{gorev.Baslik}' kodlu gÃ¶rev planlamasÄ± onayÄ±na sunuldu.");

            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> ReviewPlan(int id, string karar, string? onayNotu)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                return Unauthorized();
            }

            gorev.OnayDurumu = karar == "Onayla" ? "OnaylandÄ±" : "Reddedildi";
            gorev.OnayNotu = onayNotu;
            
            await _context.SaveChangesAsync();

            string logMessage = karar == "Onayla" 
                ? $"'{gorev.Baslik}' gÃ¶rev planlamasÄ± onaylandÄ±."
                : $"'{gorev.Baslik}' gÃ¶rev planÄ± reddedildi. Not: {onayNotu}";

            await _logService.LogAsync(currentUserId, "Plan DeÄŸerlendirildi", logMessage);

            return RedirectToAction(nameof(Index));
        }
    }
}
