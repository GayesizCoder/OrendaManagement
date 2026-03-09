using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("Kisiler", Schema = "gayemkaratas_OrendaAdmin")]
    public class Kullanici
    {
        [Key]
        public int CalisanID { get; set; } // SQL'deki Primary Key

        public int RolID { get; set; } // Veritabanında zorunlu alan

        [Required(ErrorMessage = "Ad zorunludur")]
        [MaxLength(50)]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur")]
        [MaxLength(50)]
        public string Soyad { get; set; }

        [MaxLength(100)]
        public string? Eposta { get; set; } // SQL'de NULL olabilir

        [MaxLength(15)]
        public string? Telefon { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [MaxLength(100)]
        public string KullaniciAdi { get; set; }

        [Column("SifreHash")] // SQL'deki gerçek kolon ismi
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Sifre { get; set; }

        [MaxLength(50)]
        public string? RfidID { get; set; }
        
        [MaxLength(50)]
        public string? SonGirisIP { get; set; }

        [MaxLength(50)]
        public string? CihazTipi { get; set; }

        public int? TakimID { get; set; }
        [ForeignKey("TakimID")]
        public virtual Takim? Takim { get; set; }

        public int? DepartmanID { get; set; }
        [ForeignKey("DepartmanID")]
        public virtual Departman? Departman { get; set; }

        [MaxLength(50)]
        public string AktiflikDurumu { get; set; } = "Çevrimdışı";

        public double HaftalikVerimlilikSkoru { get; set; } = 0;
    }
}