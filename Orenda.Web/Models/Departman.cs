using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("Departmanlar", Schema = "gayemkaratas_OrendaAdmin")]
    public class Departman
    {
        [Key]
        public int DepartmanID { get; set; }

        [Required(ErrorMessage = "Departman adı zorunludur")]
        public string Ad { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // Navigation Property: A department can have multiple teams
        public virtual ICollection<Takim> Takimlar { get; set; } = new List<Takim>();
        
        // Navigation Property: A department can have multiple employees directly if needed
        public virtual ICollection<Kullanici> Calisanlar { get; set; } = new List<Kullanici>();
    }
}
