using System;
using System.Collections.Generic;
using Orenda.Web.Models;

namespace Orenda.Web.Models.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        public Kullanici KullaniciBilgisi { get; set; } = null!;
        
        // Görev İstatistikleri
        public int ToplamGorev { get; set; }
        public int TamamlananGorev { get; set; }
        public int DevamEdenGorev { get; set; }
        public int YapilacakGorev { get; set; }
        
        // Son Görevler Listesi
        public List<ToDo> GuncelGorevler { get; set; } = new List<ToDo>();
        
        // Takım Arkadaşları
        public List<Kullanici> TakimArkadaslari { get; set; } = new List<Kullanici>();
    }
}
