using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("Takimlar", Schema = "gayemkaratas_OrendaAdmin")]
    public class Takim
    {
        [Key]
        public int TakimID { get; set; }

        // Optional link to a Department
        public int? DepartmanID { get; set; }
        
        [ForeignKey("DepartmanID")]
        public virtual Departman? Departman { get; set; }

        [Required(ErrorMessage = "Takım adı zorunludur")]
        public string Ad { get; set; }

        public string? Aciklama { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // Navigation Property: A team can have many employees
        public virtual ICollection<Kullanici> Uyeler { get; set; }
    }
}
