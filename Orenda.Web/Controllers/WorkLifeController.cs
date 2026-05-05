using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orenda.Web.Controllers
{
    [Authorize]
    public class WorkLifeController : Controller
    {
        private readonly OrendaDbContext _context;

        public WorkLifeController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Kullanicilar
                .Include(u => u.Takim)
                .FirstOrDefaultAsync(u => u.CalisanID == userId);

            if (user == null) return NotFound();

            var ucAyOnce = DateTime.Now.AddMonths(-3);

            var izinler = await _context.Izinler
                .Where(i => i.CalisanID == userId)
                .OrderByDescending(i => i.BaslangicTarihi)
                .ToListAsync();

            var talepler = await _context.Talepler
                .Where(t => t.CalisanID == userId)
                .OrderByDescending(t => t.OlusturulmaTarihi)
                .ToListAsync();

            // İzin günlerini hesapla (Tüm geçmişe bakarak dengeyi koruyoruz)
            int kullanilanIzin = izinler
                .Where(i => i.Durum == "Onaylandı")
                .Sum(i => (i.BitisTarihi - i.BaslangicTarihi).Days + 1);

            ViewBag.KalanIzin = user.YillikIzinHakki - kullanilanIzin;
            ViewBag.YenilenmeTarihi = user.IseBaslamaTarihi.AddYears(DateTime.Now.Year - user.IseBaslamaTarihi.Year + (DateTime.Now > user.IseBaslamaTarihi.AddYears(DateTime.Now.Year - user.IseBaslamaTarihi.Year) ? 1 : 0));
            
            // Sadece son 3 ayı veya gelecek kayıtları gönder
            ViewBag.Izinler = izinler.Where(i => i.BaslangicTarihi >= ucAyOnce).ToList();
            ViewBag.Talepler = talepler.Where(t => t.OlusturulmaTarihi >= ucAyOnce).ToList();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> RequestLeave(Izin model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            model.CalisanID = userId;
            model.Durum = "Onay Bekliyor";
            
            if (ModelState.IsValid)
            {
                _context.Izinler.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "İzin talebiniz başarıyla iletildi.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(Talep model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            model.CalisanID = userId;
            model.Durum = "Beklemede";
            model.OlusturulmaTarihi = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Talepler.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Talebiniz yönetime iletildi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
