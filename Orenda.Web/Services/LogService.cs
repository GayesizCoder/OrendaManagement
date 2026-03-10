using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Orenda.Web.Data;
using Orenda.Web.Models;

namespace Orenda.Web.Services
{
    public class LogService : ILogService
    {
        private readonly OrendaDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(OrendaDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(int kullaniciId, string islemTipi, string islemDetayi)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Bilinmiyor";

            var logEntry = new SistemLog
            {
                KullaniciID = kullaniciId,
                IslemTipi = islemTipi,
                IslemDetayi = islemDetayi,
                IPAdresi = ipAddress,
                IslemTarihi = DateTime.Now
            };

            _context.SistemLoglari.Add(logEntry);
            await _context.SaveChangesAsync();
        }
    }
}
