using Microsoft.AspNetCore.Mvc;
using Orenda.Web.Data;
using System.Linq;

namespace Orenda.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly OrendaDbContext _context;

        public AccountController(OrendaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string kullaniciAdi, string sifre)
        {
            // Yeni model ismine (Kullanicilar) göre sorgu güncellendi
            var user = _context.Kullanicilar.FirstOrDefault(u => u.KullaniciAdi == kullaniciAdi && u.Sifre == sifre);

            if (user != null)
            {
                // Giriş yapan cihazın IP adresini yakala
                string remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Veritabanındaki IP bilgisini güncelle
                user.SonGirisIP = remoteIpAddress;
                user.CihazTipi = "Web"; // Bu endpoint web login için

                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }
    }
}