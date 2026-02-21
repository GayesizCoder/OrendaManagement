using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    // Veritabanındaki 'Kisiler' tablosuna ve doğru şemaya bağladık
    [Table("Kisiler", Schema = "gayemkaratas_OrendaAdmin")]
    public class Kullanici
    {
        [Key]
        public int CalisanID { get; set; } // SQL betiğinizdeki Primary Key ile eşleşti

        [Required]
        public string KullaniciAdi { get; set; }

        [Required]
        public string Sifre { get; set; }

        public string? Ad { get; set; }
        public string? Soyad { get; set; }

        // Akıllı saat/Kart girişi için (WinForms ile koordinasyon)
        public string? RfidID { get; set; }

        // --- Cihaz ve IP Takibi İçin Yeni Alanlar ---
        public string? SonGirisIP { get; set; } // Hangi IP'den bağlanıldı?
        public string? CihazTipi { get; set; } // Web mi, Akıllı Saat mi?
        public string? BagliOlduguPC { get; set; } // Saatin bağlı olduğu terminal IP'si
    }
}