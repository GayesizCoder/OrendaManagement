using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using Orenda.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orenda.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly OrendaDbContext _context;
        private readonly ILogService _logService;

        public AdminController(OrendaDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<IActionResult> Management()
        {
            // Dashboard data for the management screen
            ViewBag.PendingTasksCount = await _context.ToDos.CountAsync(t => t.OnayDurumu == "Onay Bekliyor");
            ViewBag.PendingLeavesCount = await _context.Izinler.CountAsync(i => i.Durum == "Onay Bekliyor");
            ViewBag.PendingRequestsCount = await _context.Talepler.CountAsync(t => t.Durum == "Beklemede");
            ViewBag.TotalEmployees = await _context.Kullanicilar.CountAsync();
            ViewBag.TotalDevices = await _context.Cihazlar.CountAsync();

            // Gerçek bekleyen talepleri al (Yapay veri yerine)
            ViewBag.RecentRequests = await _context.Talepler
                .Include(t => t.Calisan)
                .Where(t => t.Durum == "Beklemede")
                .OrderByDescending(t => t.OlusturulmaTarihi)
                .Take(3)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> TaskApprovals()
        {
            var pendingTasks = await _context.ToDos
                .Include(t => t.AtananKisi)
                .Include(t => t.Takim)
                .Where(t => t.OnayDurumu == "Onay Bekliyor")
                .OrderByDescending(t => t.BaslangicTarihi)
                .ToListAsync();

            return View(pendingTasks);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewTask(int id, string karar, string? onayNotu)
        {
            var gorev = await _context.ToDos.FindAsync(id);
            if (gorev == null) return NotFound();

            gorev.OnayDurumu = karar == "Onayla" ? "Onaylandı" : "Reddedildi";
            gorev.OnayNotu = onayNotu;
            
            await _context.SaveChangesAsync();

            // Log the action
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            string logMessage = karar == "Onayla" 
                ? $"'{gorev.Baslik}' görev planlaması onaylandı."
                : $"'{gorev.Baslik}' görev planı reddedildi. Not: {onayNotu}";
            await _logService.LogAsync(currentUserId, "Görev Değerlendirildi", logMessage);

            return RedirectToAction(nameof(TaskApprovals));
        }

        public async Task<IActionResult> LeaveApprovals()
        {
            var pendingLeaves = await _context.Izinler
                .Include(i => i.Calisan)
                .OrderByDescending(i => i.BaslangicTarihi)
                .ToListAsync();

            return View(pendingLeaves);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewLeave(int id, string karar, string? yoneticiNotu)
        {
            var izin = await _context.Izinler.FindAsync(id);
            if (izin == null) return NotFound();

            izin.Durum = karar == "Onayla" ? "Onaylandı" : "Reddedildi";
            izin.YöneticiNotu = yoneticiNotu;
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LeaveApprovals));
        }

        public async Task<IActionResult> RequestManagement()
        {
            var talepler = await _context.Talepler
                .Include(t => t.Calisan)
                .OrderByDescending(t => t.OlusturulmaTarihi)
                .ToListAsync();

            return View(talepler);
        }

        [HttpPost]
        public async Task<IActionResult> RespondToRequest(int id, string yanit, string? karar)
        {
            var talep = await _context.Talepler.FindAsync(id);
            if (talep == null) return NotFound();

            talep.Yanit = yanit;
            talep.Durum = karar == "Reddet" ? "Reddedildi" : "Yanıtlandı";
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(RequestManagement));
        }

        public async Task<IActionResult> DeviceManagement()
        {
            var cihazlar = await _context.Cihazlar
                .Include(c => c.AtananCalisan)
                .OrderBy(c => c.CihazAdi)
                .ToListAsync();
            
            ViewBag.Employees = await _context.Kullanicilar.OrderBy(k => k.Ad).ToListAsync();
            return View(cihazlar);
        }

        [HttpPost]
        public async Task<IActionResult> AddDevice(Cihaz model)
        {
            if (ModelState.IsValid)
            {
                _context.Cihazlar.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DeviceManagement));
        }

        [HttpPost]
        public async Task<IActionResult> AssignDevice(int cihazId, int? calisanId)
        {
            var cihaz = await _context.Cihazlar.FindAsync(cihazId);
            if (cihaz != null)
            {
                cihaz.AtananCalisanID = calisanId;
                cihaz.Durum = calisanId.HasValue ? "Kullanımda" : "Müsait";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(DeviceManagement));
        }
    }
}
