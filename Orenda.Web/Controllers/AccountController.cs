using Microsoft.AspNetCore.Mvc;
using Orenda.Web.Data;
using Orenda.Web.Models;
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
            // Hem kullanıcı adı hem e-posta ile giriş desteği
            var user = _context.Kullanicilar.FirstOrDefault(u =>
                (u.KullaniciAdi == kullaniciAdi || (u.Eposta != null && u.Eposta == kullaniciAdi))
                && u.Sifre == sifre);

            if (user != null)
            {
                user.SonGirisIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Giriş bilgileri hatalı!";
            return View();
        }

        [HttpGet]
        public IActionResult Register() => View();


        [HttpPost]
        public IActionResult Register(Kullanici yeniKullanici)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adı veya E-posta kontrolü
                var mevcutMu = _context.Kullanicilar.Any(u =>
                    u.KullaniciAdi == yeniKullanici.KullaniciAdi ||
                    (yeniKullanici.Eposta != null && u.Eposta == yeniKullanici.Eposta));

                if (mevcutMu)
                {
                    ViewBag.Error = "Bu kullanıcı adı veya e-posta zaten kullanımda!";
                    return View(yeniKullanici);
                }

                // Veritabanında RolID zorunlu olduğu için varsayılan "Personel" rolünü (örn: 2) atıyoruz
                yeniKullanici.RolID = 2;

                _context.Kullanicilar.Add(yeniKullanici);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(yeniKullanici);
        }
    }
}