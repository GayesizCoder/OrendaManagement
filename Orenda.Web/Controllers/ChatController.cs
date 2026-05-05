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
    public class ChatController : Controller
    {
        private readonly OrendaDbContext _context;

        public ChatController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            // Mevcut sohbetleri getir
            var chatPartners = await _context.Mesajlar
                .Where(m => m.GonderenID == userId || m.AliciID == userId)
                .Select(m => m.GonderenID == userId ? m.AliciID : m.GonderenID)
                .Distinct()
                .ToListAsync();

            // Onaylı istekleri olanları da ekle
            var approvedRequests = await _context.SohbetIstekleri
                .Where(s => (s.GonderenID == userId || s.AliciID == userId) && s.Durum == "Onaylandı")
                .Select(s => s.GonderenID == userId ? s.AliciID : s.GonderenID)
                .ToListAsync();

            var allPartners = chatPartners.Union(approvedRequests).Distinct().ToList();

            // Yöneticileri ve Ekip Arkadaşlarını ekle (Default)
            var user = await _context.Kullanicilar.FindAsync(userId);
            if (user != null)
            {
                var admins = await _context.Kullanicilar.Where(k => k.RolID == 1 && k.CalisanID != userId).Select(k => k.CalisanID).ToListAsync();
                var teamMates = await _context.Kullanicilar.Where(k => k.TakimID == user.TakimID && k.CalisanID != userId).Select(k => k.CalisanID).ToListAsync();
                allPartners = allPartners.Union(admins).Union(teamMates).Distinct().ToList();
            }

            var partners = await _context.Kullanicilar
                .Where(k => allPartners.Contains(k.CalisanID))
                .ToListAsync();

            ViewBag.Partners = partners;
            ViewBag.SelectedPartnerId = id;
            
            // Bekleyen istekler
            ViewBag.PendingRequests = await _context.SohbetIstekleri
                .Include(s => s.Gonderen)
                .Where(s => s.AliciID == userId && s.Durum == "Beklemede")
                .ToListAsync();

            if (id.HasValue)
            {
                var messages = await _context.Mesajlar
                    .Where(m => (m.GonderenID == userId && m.AliciID == id) || (m.GonderenID == id && m.AliciID == userId))
                    .OrderBy(m => m.GonderilmeTarihi)
                    .ToListAsync();
                
                ViewBag.SelectedPartner = await _context.Kullanicilar.FindAsync(id.Value);
                return View(messages);
            }

            return View(new System.Collections.Generic.List<Mesaj>());
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int aliciId, string icerik, string? fotografUrl)
        {
            var gonderenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var mesaj = new Mesaj
            {
                GonderenID = gonderenId,
                AliciID = aliciId,
                Icerik = icerik,
                FotografUrl = fotografUrl,
                GonderilmeTarihi = DateTime.Now
            };

            _context.Mesajlar.Add(mesaj);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = aliciId });
        }

        [HttpPost]
        public async Task<IActionResult> SendChatRequest(string globalId)
        {
            var gonderenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var alici = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.GlobalID == globalId);

            if (alici == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (alici.CalisanID == gonderenId)
            {
                TempData["Error"] = "Kendinize istek gönderemezsiniz.";
                return RedirectToAction(nameof(Index));
            }

            var existingRequest = await _context.SohbetIstekleri
                .FirstOrDefaultAsync(s => (s.GonderenID == gonderenId && s.AliciID == alici.CalisanID) || 
                                         (s.GonderenID == alici.CalisanID && s.AliciID == gonderenId));

            if (existingRequest != null)
            {
                TempData["Error"] = "Zaten bir istek mevcut.";
                return RedirectToAction(nameof(Index));
            }

            var istek = new SohbetIstegi
            {
                GonderenID = gonderenId,
                AliciID = alici.CalisanID,
                Durum = "Beklemede"
            };

            _context.SohbetIstekleri.Add(istek);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sohbet isteği gönderildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> HandleRequest(int istekId, string karar)
        {
            var istek = await _context.SohbetIstekleri.FindAsync(istekId);
            if (istek != null)
            {
                istek.Durum = karar == "Onayla" ? "Onaylandı" : "Reddedildi";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
