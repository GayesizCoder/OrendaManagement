using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Orenda.Web.Services;

namespace Orenda.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeamController : Controller
    {
        private readonly OrendaDbContext _context;
        private readonly ILogService _logService;

        public TeamController(OrendaDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        // 1. GENEL BAKIŞ (INDEX)
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalDepartments = await _context.Departmanlar.CountAsync();
            ViewBag.TotalTeams = await _context.Takimlar.CountAsync();
            ViewBag.TotalEmployees = await _context.Kullanicilar.CountAsync();
            ViewBag.UnassignedEmployees = await _context.Kullanicilar.CountAsync(k => k.DepartmanID == null && k.TakimID == null && k.RolID != 1);

            return View();
        }

        // 2. DEPARTMANLAR
        public async Task<IActionResult> Departments()
        {
            var departmanlar = await _context.Departmanlar.Include(d => d.Takimlar).ToListAsync();
            return View(departmanlar);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment(string Ad, string? Aciklama)
        {
            if (!string.IsNullOrWhiteSpace(Ad))
            {
                var yeniDepartman = new Departman { Ad = Ad, Aciklama = Aciklama, OlusturmaTarihi = DateTime.Now };
                _context.Departmanlar.Add(yeniDepartman);
                await _context.SaveChangesAsync();
                
                // Get current Admin UserId
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
                
                await _logService.LogAsync(currentUserId, "Departman Eklendi", $"'{Ad}' adında yeni bir departman oluşturuldu.");
            }
            return RedirectToAction(nameof(Departments));
        }

        // 3. TAKIMLAR
        public async Task<IActionResult> Teams()
        {
            var takimlar = await _context.Takimlar.Include(t => t.Departman).Include(t => t.Uyeler).ToListAsync();
            ViewBag.Departmanlar = await _context.Departmanlar.ToListAsync();
            return View(takimlar);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeam(string Ad, string? Aciklama, int? DepartmanID)
        {
            if (!string.IsNullOrWhiteSpace(Ad))
            {
                var yeniTakim = new Takim { Ad = Ad, Aciklama = Aciklama, DepartmanID = DepartmanID, OlusturmaTarihi = DateTime.Now };
                _context.Takimlar.Add(yeniTakim);
                await _context.SaveChangesAsync();
                
                // Get current Admin UserId
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
                
                await _logService.LogAsync(currentUserId, "Takım Eklendi", $"'{Ad}' adında yeni bir takım oluşturuldu.");
            }
            return RedirectToAction(nameof(Teams));
        }

        // 4. ÇALIŞANLAR
        public async Task<IActionResult> Employees()
        {
            var calisanlar = await _context.Kullanicilar
                .Include(k => k.Departman)
                .Include(k => k.Takim)
                .OrderBy(k => k.Ad)
                .ToListAsync();

            ViewBag.Departmanlar = new SelectList(await _context.Departmanlar.ToListAsync(), "DepartmanID", "Ad");
            ViewBag.Takimlar = new SelectList(await _context.Takimlar.ToListAsync(), "TakimID", "Ad");

            return View(calisanlar);
        }

        [HttpPost]
        public async Task<IActionResult> AssignEmployee(int CalisanID, int? DepartmanID, int? TakimID)
        {
            var calisan = await _context.Kullanicilar.FindAsync(CalisanID);
            if (calisan != null)
            {
                calisan.DepartmanID = DepartmanID;
                calisan.TakimID = TakimID;
                await _context.SaveChangesAsync();
                
                // Get current Admin UserId
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
                
                await _logService.LogAsync(currentUserId, "Çalışan Ataması", $"{calisan.Ad} {calisan.Soyad} (ID: {CalisanID}) adlı personelin departman/takım bilgileri güncellendi.");
            }
            return RedirectToAction(nameof(Employees));
        }
    }
}
