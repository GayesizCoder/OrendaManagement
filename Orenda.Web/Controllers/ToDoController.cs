using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orenda.Web.Data;
using Orenda.Web.Models;

namespace Orenda.Web.Controllers
{
    public class ToDoController : Controller
    {
        private readonly OrendaDbContext _context;

        public ToDoController(OrendaDbContext context)
        {
            _context = context;
        }

        // Görevleri listeleme metodu burada olur
        public async Task<IActionResult> Index()
        {
            // Include(t => t.AtananKisi) ile görevli adını da çekiyoruz
            var gorevler = await _context.ToDos
                                         .Include(t => t.AtananKisi)
                                         .ToListAsync();
            return View(gorevler);
        }
    }
}