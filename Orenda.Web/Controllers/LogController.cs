using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Orenda.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogController : Controller
    {
        private readonly OrendaDbContext _context;

        public LogController(OrendaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _context.SistemLoglari
                .Include(l => l.Kullanici)
                .OrderByDescending(l => l.IslemTarihi)
                .Take(200) // Performans için son 200 logu getir
                .ToListAsync();

            return View(logs);
        }
    }
}
