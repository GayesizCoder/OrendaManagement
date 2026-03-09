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
    }
}
