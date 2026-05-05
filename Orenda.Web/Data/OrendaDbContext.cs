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
        public DbSet<SistemLog> SistemLoglari { get; set; }
        public DbSet<SaglikVerisi> SaglikVerileri { get; set; }
        public DbSet<GorevAdimi> GorevAdimlari { get; set; }
        public DbSet<Izin> Izinler { get; set; }
        public DbSet<Talep> Talepler { get; set; }
        public DbSet<Cihaz> Cihazlar { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<SohbetIstegi> SohbetIstekleri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mesajlar İlişkileri
            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Gonderen)
                .WithMany()
                .HasForeignKey(m => m.GonderenID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Alici)
                .WithMany()
                .HasForeignKey(m => m.AliciID)
                .OnDelete(DeleteBehavior.Restrict);

            // Sohbet İstekleri İlişkileri
            modelBuilder.Entity<SohbetIstegi>()
                .HasOne(s => s.Gonderen)
                .WithMany()
                .HasForeignKey(s => s.GonderenID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SohbetIstegi>()
                .HasOne(s => s.Alici)
                .WithMany()
                .HasForeignKey(s => s.AliciID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
