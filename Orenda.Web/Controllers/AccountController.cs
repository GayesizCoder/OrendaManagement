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
            var user = _context.Kisiler.FirstOrDefault(u => u.KullaniciAdi == kullaniciAdi && u.Sifre == sifre);

            if (user != null)
            {
                // Giriş başarılıysa Dashboard'a yönlendir
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }
    }
}