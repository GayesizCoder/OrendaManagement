using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("SistemLoglari", Schema = "gayemkaratas_OrendaAdmin")]
    public class SistemLog
    {
        [Key]
        public int LogID { get; set; }

        public int KullaniciID { get; set; } // İşlemi yapan kişi

        [Required]
        [MaxLength(100)]
        public string IslemTipi { get; set; } // Örn: "Görev Eklendi", "Sisteme Giriş"

        [MaxLength(500)]
        public string IslemDetayi { get; set; } // Örn: "Yeni bir yazılım görevi oluşturuldu."
        
        [MaxLength(50)]
        public string IPAdresi { get; set; } // Logu tetikleyen network IP adresi

        public DateTime IslemTarihi { get; set; } = DateTime.Now;

        // Navigation Property: Olayı gerçekleştiren kişi
        [ForeignKey("KullaniciID")]
        public virtual Kullanici Kullanici { get; set; }
    }
}
