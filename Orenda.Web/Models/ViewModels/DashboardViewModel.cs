using System;
using System.Collections.Generic;
using Orenda.Web.Models;

namespace Orenda.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int ToplamKullanici { get; set; }
        public int AktifCevrimici { get; set; }
        
        // Görev İstatistikleri
        public int ToplamGorev { get; set; }
        public int TamamlananGorev { get; set; }
        public double AktifGorevYuzdesi { get; set; }
        
        public double GelistirmeYuzdesi { get; set; }
        public double TasarimYuzdesi { get; set; }
        public double DigerYuzdesi { get; set; }

        public List<ToDo> HaftalikProjeler { get; set; } = new List<ToDo>();

        // Yeni eklenen gerçek veriler
        public List<SistemLog> SonAktiviteler { get; set; } = new List<SistemLog>();
        public Dictionary<string, int> DepartmanDagilimi { get; set; } = new Dictionary<string, int>();
        
        public SistemLog? SonGirisYapan { get; set; }
        
        // Departman Dağılım Toplamı
        public int DepartmanDoluKullaniciSayisi { get; set; }
    }
}
