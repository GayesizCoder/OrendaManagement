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

            IQueryable<ToDo> gorevSorgusu = _context.ToDos.Include(t => t.AtananKisi);

            if (!isAdmin)
            {
                // Eğer Admin değilse sadece kendine atananları görsün
                gorevSorgusu = gorevSorgusu.Where(t => t.AtananCalisanID == currentUserId);
            }

            var gorevler = await gorevSorgusu.OrderByDescending(g => g.BaslangicTarihi).ToListAsync();
            
            // Eğer Admin ise Görev Atama Modal'ı için çalışan listesini gönder
            if (isAdmin)
            {
                ViewBag.Calisanlar = new SelectList(await _context.Kullanicilar.OrderBy(k => k.Ad).ToListAsync(), "CalisanID", "Ad");
            }

            return View(gorevler);
        }

        // --- ADMIN SADECE ---

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
    }
}