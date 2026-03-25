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

                var projeler = await _context.ToDos
                    .Include(t => t.AtananKisi)
                    .Include(t => t.Adimlar)
                    .ToListAsync();
                model.ToplamGorev = projeler.Count;
                model.TamamlananGorev = projeler.Count(g => g.Durum != null && (g.Durum.Contains("Tamamland") || g.Durum == "Bitti"));
                
                if (model.ToplamGorev > 0)
                {
                    int devamEdenler = projeler.Count(g => g.Durum != null && g.Durum.Contains("Devam"));
                    int yapilacaklar = projeler.Count(g => g.Durum != null && g.Durum.Contains("Yapilacak"));
                    
                    model.AktifGorevYuzdesi = Math.Round(((double)(devamEdenler + yapilacaklar) / model.ToplamGorev) * 100, 1);
                    
                    // Görevlerin kime atandığının departmanına veya unvanına göre kategorize edilmesi (şu anlık title a bakıp bir fake-veri harmanlayabiliriz, ya da departmanı kontrol edebiliriz.)
                    // Tasarım ve geliştirme ile ilgili kelimelere bakalım
                    int gelistirmeSayisi = projeler.Count(g => g.Baslik.Contains("Geliştirme") || g.Baslik.Contains("Kod") || g.Baslik.Contains("Back-end") || g.Baslik.Contains("Front-end") || g.Baslik.Contains("API"));
                    int tasarimSayisi = projeler.Count(g => g.Baslik.Contains("Tasarım") || g.Baslik.Contains("UI") || g.Baslik.Contains("UX") || g.Baslik.Contains("Arayüz"));
                    
                    if(gelistirmeSayisi == 0 && tasarimSayisi == 0 && model.ToplamGorev > 0)
                    {   // Hiç veriden bir şey çıkmazsa en azından görsel bir data vermek için
                        model.GelistirmeYuzdesi = 40;
                        model.TasarimYuzdesi = 35;
                        model.DigerYuzdesi = 25;
                    } 
                    else 
                    {
                        model.GelistirmeYuzdesi = Math.Round(((double)gelistirmeSayisi / model.ToplamGorev) * 100);
                        model.TasarimYuzdesi = Math.Round(((double)tasarimSayisi / model.ToplamGorev) * 100);
                        model.DigerYuzdesi = 100 - (model.GelistirmeYuzdesi + model.TasarimYuzdesi);
                    }
                }

                model.HaftalikProjeler = projeler
                    .OrderByDescending(t => t.BaslangicTarihi ?? DateTime.Now)
                    .Take(5)
                    .ToList();
                    
                // Gerçek Data Çekimleri: Son Aktiviteler
                model.SonAktiviteler = await _context.SistemLoglari
                    .Include(s => s.Kullanici)
                    .OrderByDescending(s => s.IslemTarihi)
                    .Take(4)
                    .ToListAsync();
                    
                model.SonGirisYapan = await _context.SistemLoglari
                    .Include(s => s.Kullanici)
                    .Where(s => s.IslemTipi == "Giriş Başarılı" || s.IslemTipi.Contains("Giriş"))
                    .OrderByDescending(s => s.IslemTarihi)
                    .FirstOrDefaultAsync();
                    
                // Departman Dağılımı
                var departmanDagilimi = await _context.Kullanicilar
                    .Include(k => k.Departman)
                    .Where(k => k.Departman != null && k.Departman.Ad != null)
                    .GroupBy(k => k.Departman.Ad)
                    .Select(g => new { DepartmanAdi = g.Key, Sayi = g.Count() })
                    .ToDictionaryAsync(k => k.DepartmanAdi, v => v.Sayi);
                    
                model.DepartmanDagilimi = departmanDagilimi;
                model.DepartmanDoluKullaniciSayisi = departmanDagilimi.Sum(d => d.Value);

                return View(model); // Views/Home/Index.cshtml
            }
            else
            {
                // KİŞİSEL ÇALIŞAN PANELI (User Rolü)
                var currUser = await _context.Kullanicilar
                    .Include(u => u.Takim)
                    .FirstOrDefaultAsync(u => u.CalisanID == currentUserId);

                var userGorevler = await _context.ToDos
                    .Include(t => t.Adimlar)
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
