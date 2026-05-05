using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    public class Cihaz
    {
        [Key]
        public int CihazID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CihazAdi { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string SeriNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Tür { get; set; } = string.Empty; // Laptop, Telefon, Monitör, Wearable

        public int? AtananCalisanID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Durum { get; set; } = "Müsait"; // Müsait, Kullanımda, Arızalı, Bakımda

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        [ForeignKey("AtananCalisanID")]
        public virtual Kullanici? AtananCalisan { get; set; }
    }
}
