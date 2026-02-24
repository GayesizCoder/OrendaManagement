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
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; }

        public string? Eposta { get; set; } // SQL'de NULL olabilir

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string KullaniciAdi { get; set; }

        [Column("SifreHash")] // SQL'deki gerçek kolon ismi
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Sifre { get; set; }

        public string? RfidID { get; set; }
        public string? SonGirisIP { get; set; }
        public string? CihazTipi { get; set; }
    }
}