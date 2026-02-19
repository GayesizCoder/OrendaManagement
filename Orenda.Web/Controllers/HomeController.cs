using Microsoft.AspNetCore.Mvc;
using Orenda.Web.Data;
using System.Linq;

namespace Orenda.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrendaDbContext _context;

        public HomeController(OrendaDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            // Buluttaki ToDo tablosundan gerçek verileri çekiyoruz
            var gorevler = _context.ToDos.ToList();

            // Grafik için gerçek durum sayýlarýný hesaplýyoruz
            ViewBag.Tamamlanan = gorevler.Count(x => x.Durum == "Tamamlandi");
            ViewBag.DevamEden = gorevler.Count(x => x.Durum == "Devam Ediyor");
            ViewBag.Bekleyen = gorevler.Count(x => x.Durum == "Yapilacak");

            return View(gorevler);
        }
    }

}