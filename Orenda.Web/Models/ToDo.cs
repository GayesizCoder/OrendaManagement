using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("ToDo", Schema = "gayemkaratas_OrendaAdmin")]
    public class ToDo
    {
        [Key]
        public int GorevNo { get; set; } // SQL'deki Primary Key

        public int AtananCalisanID { get; set; } // Foreign Key

        [Required(ErrorMessage = "Başlık boş bırakılamaz")]
        public string Baslik { get; set; }

        public string? Aciklama { get; set; }

        public string Durum { get; set; } = "Yapilacak"; // Varsayılan durum

        public DateTime? BitisTarihi { get; set; }

        // Navigation Property: Görevin kime ait olduğunu kod içinde kolayca görmek için
        [ForeignKey("AtananCalisanID")]
        public virtual Kullanici? AtananKisi { get; set; }
    }
}