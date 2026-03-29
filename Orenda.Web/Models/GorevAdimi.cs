using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("GorevAdimlari", Schema = "gayemkaratas_OrendaAdmin")]
    public class GorevAdimi
    {
        [Key]
        public int AdimID { get; set; }

        public int GorevNo { get; set; } // Foreign Key to ToDo

        [Required(ErrorMessage = "Adım başlığı gereklidir")]
        [MaxLength(150)]
        public string Baslik { get; set; } = string.Empty;

        public bool TamamlandiMi { get; set; } = false;

        // Kullanıcı bu iş parçasının görevin % kaçını oluşturduğunu belirtebilir.
        // Eğer null ise, otomatik eşit paylaştırılır.
        public int? AgirlikYuzdesi { get; set; }

        [ForeignKey("GorevNo")]
        public virtual ToDo ToDoTutucu { get; set; } = null!;
    }
}
