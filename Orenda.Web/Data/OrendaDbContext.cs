using Microsoft.EntityFrameworkCore;
using Orenda.Web.Models;

namespace Orenda.Web.Data
{
    public class OrendaDbContext : DbContext
    {
        public OrendaDbContext(DbContextOptions<OrendaDbContext> options) : base(options) { }

        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Kullanici> Kisiler { get; set; }
    }
}