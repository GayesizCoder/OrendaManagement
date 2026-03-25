using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Orenda.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DeviceController : Controller
    {
        private readonly OrendaDbContext _context;

        public DeviceController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var calisanlar = await _context.Kullanicilar
                .Include(k => k.Departman)
                .Include(k => k.Takim)
                .OrderBy(k => k.Ad)
                .ToListAsync();

            return View(calisanlar);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDevice(int calisanId)
        {
            var calisan = await _context.Kullanicilar.FindAsync(calisanId);
            if (calisan != null)
            {
                calisan.RfidID = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
