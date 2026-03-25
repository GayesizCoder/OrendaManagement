using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using System.Security.Claims;

namespace Orenda.Web.Controllers
{
    [Authorize]
    public class LifeController : Controller
    {
        private readonly OrendaDbContext _context;

        public LifeController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get today's health data for the logged-in user
            var today = DateTime.Today;
            var healthData = await _context.SaglikVerileri
                .Where(s => s.CalisanID == userId && s.TarihSaat >= today)
                .OrderBy(s => s.TarihSaat)
                .ToListAsync();

            // Calculate summaries for the UI
            var currentHeartRate = healthData.LastOrDefault()?.Nabiz ?? 0;
            var totalStepsToday = healthData.OrderByDescending(s => s.TarihSaat).FirstOrDefault()?.AdimSayisi ?? 0; // Assuming total steps is the latest reading

            // Pass history to chart
            ViewBag.HeartRateHistory = healthData.Select(s => s.Nabiz).ToList();
            ViewBag.HeartRateLabels = healthData.Select(s => s.TarihSaat.ToString("HH:mm")).ToList();

            ViewBag.CurrentHeartRate = currentHeartRate;
            ViewBag.TotalSteps = totalStepsToday;
            ViewBag.StepGoal = 10000; // Mock goal

            return View();
        }

        // Simülasyon / Test amaçlı buton: Manuel nabız ve adım eklemek için
        [HttpPost]
        public async Task<IActionResult> GuncelleTestVerisi()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var rng = new Random();
                var yeniVeri = new SaglikVerisi
                {
                    CalisanID = userId,
                    Nabiz = rng.Next(70, 110), // Rastgele nabız (70-110 bpm)
                    AdimSayisi = rng.Next(2000, 12000), // Rastgele günlük adım
                    TarihSaat = DateTime.Now
                };

                _context.SaglikVerileri.Add(yeniVeri);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
