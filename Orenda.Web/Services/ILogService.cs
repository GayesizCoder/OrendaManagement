using System;
using System.Threading.Tasks;

namespace Orenda.Web.Services
{
    public interface ILogService
    {
        Task LogAsync(int kullaniciId, string islemTipi, string islemDetayi);
    }
}
