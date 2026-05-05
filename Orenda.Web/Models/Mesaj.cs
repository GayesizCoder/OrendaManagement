using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    public class Mesaj
    {
        [Key]
        public int MesajID { get; set; }

        [Required]
        public int GonderenID { get; set; }

        [Required]
        public int AliciID { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Icerik { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FotografUrl { get; set; }

        public DateTime GonderilmeTarihi { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;

        [ForeignKey("GonderenID")]
        public virtual Kullanici? Gonderen { get; set; }

        [ForeignKey("AliciID")]
        public virtual Kullanici? Alici { get; set; }
    }
}
