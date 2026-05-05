using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    public class SohbetIstegi
    {
        [Key]
        public int IstekID { get; set; }

        [Required]
        public int GonderenID { get; set; }

        [Required]
        public int AliciID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Durum { get; set; } = "Beklemede"; // Beklemede, Onaylandı, Reddedildi

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        [ForeignKey("GonderenID")]
        public virtual Kullanici? Gonderen { get; set; }

        [ForeignKey("AliciID")]
        public virtual Kullanici? Alici { get; set; }
    }
}
