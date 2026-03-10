using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using Orenda.Web.Models.ViewModels;

namespace Orenda.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly OrendaDbContext _context;

        public HomeController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Kullanıcı bilgilerini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
            bool isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                // ADMIN PANELI (ESKİ YAPI)
                var model = new DashboardViewModel();
                
                model.ToplamKullanici = await _context.Kullanicilar.CountAsync();
                model.AktifCevrimici = await _context.Kullanicilar.CountAsync(k => k.AktiflikDurumu == "Çevrimiçi" || k.AktiflikDurumu == "Online");

                var projeler = await _context.ToDos.ToListAsync();
                model.ToplamGorev = projeler.Count;
                model.TamamlananGorev = projeler.Count(g => g.Durum != null && (g.Durum.Contains("Tamamland") || g.Durum == "Bitti"));
                
                if (model.ToplamGorev > 0)
                {
                    int devamEdenler = projeler.Count(g => g.Durum != null && g.Durum.Contains("Devam"));
                    int yapilacaklar = projeler.Count(g => g.Durum != null && g.Durum.Contains("Yapilacak"));
                    
                    model.AktifGorevYuzdesi = Math.Round(((double)(devamEdenler + yapilacaklar) / model.ToplamGorev) * 100, 1);
                    
                    model.GelistirmeYuzdesi = 48; // Örnek Sabit
                    model.TasarimYuzdesi = 32;    // Örnek Sabit
                    model.DigerYuzdesi = 20;      // Örnek Sabit
                }

                model.HaftalikProjeler = await _context.ToDos
                    .Include(t => t.AtananKisi)
                    .OrderByDescending(t => t.BaslangicTarihi ?? DateTime.Now)
                    .Take(5)
                    .ToListAsync();

                return View(model); // Views/Home/Index.cshtml
            }
            else
            {
                // KİŞİSEL ÇALIŞAN PANELI (User Rolü)
                var currUser = await _context.Kullanicilar
                    .Include(u => u.Takim)
                    .FirstOrDefaultAsync(u => u.CalisanID == currentUserId);

                var userGorevler = await _context.ToDos
                    .Where(t => t.AtananCalisanID == currentUserId)
                    .OrderByDescending(t => t.BaslangicTarihi ?? DateTime.Now)
                    .ToListAsync();

                var teamMembers = new List<Kullanici>();
                if (currUser?.TakimID != null)
                {
                    teamMembers = await _context.Kullanicilar
                        .Where(u => u.TakimID == currUser.TakimID && u.CalisanID != currentUserId)
                        .ToListAsync();
                }

                var empModel = new EmployeeDashboardViewModel
                {
                    KullaniciBilgisi = currUser,
                    ToplamGorev = userGorevler.Count,
                    TamamlananGorev = userGorevler.Count(g => g.Durum != null && (g.Durum.Contains("Tamamland") || g.Durum == "Bitti")),
                    DevamEdenGorev = userGorevler.Count(g => g.Durum != null && g.Durum.Contains("Devam")),
                    YapilacakGorev = userGorevler.Count(g => g.Durum != null && g.Durum.Contains("Yapilacak")),
                    GuncelGorevler = userGorevler.Take(5).ToList(),
                    TakimArkadaslari = teamMembers
                };

                return View("EmployeeDashboard", empModel);
            }
        }
    }
}
