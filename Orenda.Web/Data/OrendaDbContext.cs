using Microsoft.EntityFrameworkCore;
using Orenda.Web.Models;

namespace Orenda.Web.Data
{
    public class OrendaDbContext : DbContext
    {
        public OrendaDbContext(DbContextOptions<OrendaDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Departman> Departmanlar { get; set; }
        public DbSet<Takim> Takimlar { get; set; }
    }
}
