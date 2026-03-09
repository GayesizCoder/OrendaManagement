using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using Orenda.Web.Models.ViewModels;

namespace Orenda.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrendaDbContext _context;

        public HomeController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel();

            // Ekip İstatistikleri
            model.ToplamKullanici = await _context.Kullanicilar.CountAsync();
            model.AktifCevrimici = await _context.Kullanicilar.CountAsync(k => k.AktiflikDurumu == "Çevrimiçi" || k.AktiflikDurumu == "Online");

            // Görev İstatistikleri
            var gorevler = await _context.ToDos.ToListAsync();
            model.ToplamGorev = gorevler.Count;
            model.TamamlananGorev = gorevler.Count(g => g.Durum != null && (g.Durum.Contains("Tamamland") || g.Durum == "Bitti"));
            
            if (model.ToplamGorev > 0)
            {
                int devamEdenler = gorevler.Count(g => g.Durum != null && g.Durum.Contains("Devam"));
                int yapilacaklar = gorevler.Count(g => g.Durum != null && g.Durum.Contains("Yapilacak"));
                
                model.AktifGorevYuzdesi = Math.Round(((double)(devamEdenler + yapilacaklar) / model.ToplamGorev) * 100, 1);
                
                model.GelistirmeYuzdesi = 48; // Örnek Sabit
                model.TasarimYuzdesi = 32;    // Örnek Sabit
                model.DigerYuzdesi = 20;      // Örnek Sabit
            }

            // Haftalık Projeler Timeline'a Aktarılmak İçin
            model.HaftalikProjeler = await _context.ToDos
                .Include(t => t.AtananKisi)
                .OrderByDescending(t => t.BaslangicTarihi ?? DateTime.Now)
                .Take(5)
                .ToListAsync();

            return View(model);
        }
    }
}
