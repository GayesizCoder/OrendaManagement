using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    public class Talep
    {
        [Key]
        public int TalepID { get; set; }

        [Required]
        public int CalisanID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tür { get; set; } = string.Empty; // Dilekçe, Şikayet, Öneri

        [Required]
        [MaxLength(200)]
        public string Konu { get; set; } = string.Empty;

        [Required]
        public string Mesaj { get; set; } = string.Empty;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(50)]
        public string Durum { get; set; } = "Beklemede"; // Beklemede, İnceleniyor, Yanıtlandı, Kapatıldı

        [MaxLength(500)]
        public string? Yanit { get; set; }

        [ForeignKey("CalisanID")]
        public virtual Kullanici? Calisan { get; set; }
    }
}
