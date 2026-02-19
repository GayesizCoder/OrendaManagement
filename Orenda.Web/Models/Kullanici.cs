using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("Kisiler", Schema = "gayemkaratas_OrendaAdmin")]
    public class Kullanici
    {
        [Key]
        public int KisiID { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string? AdSoyad { get; set; }
    }
}