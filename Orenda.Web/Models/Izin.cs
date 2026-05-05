using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    public class Izin
    {
        [Key]
        public int IzinID { get; set; }

        [Required]
        public int CalisanID { get; set; }

        [Required]
        public DateTime BaslangicTarihi { get; set; }

        [Required]
        public DateTime BitisTarihi { get; set; }

        [Required]
        [MaxLength(500)]
        public string Sebep { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Durum { get; set; } = "Onay Bekliyor"; // Onay Bekliyor, Onaylandı, Reddedildi

        [MaxLength(200)]
        public string? YöneticiNotu { get; set; }

        [ForeignKey("CalisanID")]
        public virtual Kullanici? Calisan { get; set; }
    }
}
