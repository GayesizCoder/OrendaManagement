using Microsoft.AspNetCore.Mvc;
using Orenda.Web.Data;
using Orenda.Web.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // PasswordHasher için eklendi
using Orenda.Web.Services;

namespace Orenda.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly OrendaDbContext _context;
        private readonly PasswordHasher<Kullanici> _passwordHasher; // Hashing class instance
        private readonly ILogService _logService;

        public AccountController(OrendaDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
            _passwordHasher = new PasswordHasher<Kullanici>();
        }

        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
        {
            // Kullanıcıyı veri tabanından 'Kullanıcı Adı' veya 'E-posta' ile bul
            var user = _context.Kullanicilar.FirstOrDefault(u =>
                u.KullaniciAdi == kullaniciAdi || (u.Eposta != null && u.Eposta == kullaniciAdi));

            if (user != null)
            {
                bool isPasswordValid = false;
                bool needsRehash = false;

                try
                {
                    // Hasher kullanarak veritabanındaki hashlenmiş şifre ile girilen düz şifreyi karşılaştır
                    var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Sifre, sifre);

                    if (passwordVerificationResult == PasswordVerificationResult.Success)
                    {
                        isPasswordValid = true;
                    }
                    else if (passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        isPasswordValid = true;
                        needsRehash = true;
                    }
                }
                catch (System.FormatException)
                {
                    // Eğer veritabanındaki şifre henüz hashlenmemişse (Eski düz metin kayıtlar)
                    if (user.Sifre == sifre)
                    {
                        isPasswordValid = true;
                        needsRehash = true; // Hash'e çevirip güncelleyeceğiz
                    }
                }

                if (isPasswordValid)
                {
                    if (needsRehash)
                    {
                        user.Sifre = _passwordHasher.HashPassword(user, sifre);
                    }
                    user.SonGirisIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                    _context.SaveChanges();

                    // Claims oluşturma
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Ad + " " + user.Soyad),
                        new Claim(ClaimTypes.NameIdentifier, user.CalisanID.ToString()),
                        new Claim(ClaimTypes.Email, user.Eposta ?? ""),
                        new Claim(ClaimTypes.Role, user.RolID == 1 ? "Admin" : "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "OrendaAuthCookie");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // "Beni Hatırla" seçeneği için ileride geliştirilebilir
                    };

                    await HttpContext.SignInAsync("OrendaAuthCookie", new ClaimsPrincipal(claimsIdentity), authProperties);
                    
                    // Log the successful login
                    await _logService.LogAsync(user.CalisanID, "Sisteme Giriş", "Kullanıcı sisteme giriş yaptı.");

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Giriş bilgileri hatalı!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("OrendaAuthCookie");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register() => View();


        [HttpPost]
        public async Task<IActionResult> Register(Kullanici yeniKullanici)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adının benzersiz olduğunu kontrol et
                bool isUsernameTaken = _context.Kullanicilar.Any(k => k.KullaniciAdi == yeniKullanici.KullaniciAdi);
                if (isUsernameTaken)
                {
                    ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten alınmış.");
                    return View(yeniKullanici);
                }

                // Veritabanında RolID zorunlu olduğu için varsayılan "Personel" rolünü (örn: 2) atıyoruz
                yeniKullanici.RolID = 2;

                // Girilen düz şifreyi Hash'leyip modele aktar
                yeniKullanici.Sifre = _passwordHasher.HashPassword(yeniKullanici, yeniKullanici.Sifre);

                _context.Kullanicilar.Add(yeniKullanici);
                await _context.SaveChangesAsync();
                
                // Log the registration
                await _logService.LogAsync(yeniKullanici.CalisanID, "Yeni Kullanıcı Oluşturuldu", $"Sisteme {yeniKullanici.KullaniciAdi} adında yeni bir kullanıcı kaydedildi.");
                
                return RedirectToAction("Login");
            }
            return View(yeniKullanici);
        }
    }
}