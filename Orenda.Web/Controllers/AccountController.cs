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
using Microsoft.EntityFrameworkCore;

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
                    user.AktiflikDurumu = "Çevrimiçi";
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
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                var user = await _context.Kullanicilar.FindAsync(userId);
                if (user != null)
                {
                    user.AktiflikDurumu = "Çevrimdışı";
                    await _context.SaveChangesAsync();
                }
            }

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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Kullanicilar
                .Include(k => k.Departman)
                .Include(k => k.Takim)
                .FirstOrDefaultAsync(k => k.CalisanID == userId);
            
            if (user == null) return NotFound();

            // Ek Veriler (Sekmeler için)
            ViewBag.SaglikVerisi = await _context.SaglikVerileri
                .Where(s => s.CalisanID == userId)
                .OrderByDescending(s => s.TarihSaat)
                .FirstOrDefaultAsync();

            ViewBag.GirisCikisLoglari = await _context.SistemLoglari
                .Where(l => l.KullaniciID == userId && (l.IslemTipi == "Sisteme Giriş" || l.IslemTipi == "Sistemden Çıkış"))
                .OrderByDescending(l => l.IslemTarihi)
                .Take(5)
                .ToListAsync();

            ViewBag.GorevIstatistik = new {
                Toplam = await _context.ToDos.CountAsync(g => g.AtananCalisanID == userId),
                Tamamlanan = await _context.ToDos.CountAsync(g => g.AtananCalisanID == userId && g.Durum == "Tamamlandı")
            };

            var model = new Orenda.Web.Models.ViewModels.ProfileViewModel
            {
                Ad = user.Ad,
                Soyad = user.Soyad,
                Eposta = user.Eposta,
                Telefon = user.Telefon,
                GlobalID = user.GlobalID
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(Orenda.Web.Models.ViewModels.ProfileViewModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Kullanicilar.FindAsync(userId);

            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.Ad = model.Ad;
                user.Soyad = model.Soyad;
                user.Eposta = model.Eposta;
                user.Telefon = model.Telefon;

                // Şifre değişikliği kontrolü
                if (!string.IsNullOrEmpty(model.YeniSifre))
                {
                    if (string.IsNullOrEmpty(model.MevcutSifre))
                    {
                        ModelState.AddModelError("MevcutSifre", "Şifre değiştirmek için mevcut şifrenizi girmelisiniz.");
                        return View(model);
                    }

                    var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Sifre, model.MevcutSifre);
                    if (verificationResult == PasswordVerificationResult.Failed)
                    {
                        ModelState.AddModelError("MevcutSifre", "Mevcut şifreniz hatalı.");
                        return View(model);
                    }

                    user.Sifre = _passwordHasher.HashPassword(user, model.YeniSifre);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                
                // Update the Identity Name in the cookie (optional, but good for UX)
                // Note: Full identity refresh usually requires re-signing in.
                
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }
    }
}