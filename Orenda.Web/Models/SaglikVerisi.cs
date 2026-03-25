using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("SaglikVerileri", Schema = "gayemkaratas_OrendaAdmin")]
    public class SaglikVerisi
    {
        [Key]
        public int SaglikVerisiID { get; set; }

        public int CalisanID { get; set; }

        public DateTime TarihSaat { get; set; } = DateTime.Now;

        public int Nabiz { get; set; }

        public int AdimSayisi { get; set; }

        public double? UykuSaati { get; set; }

        [ForeignKey("CalisanID")]
        public virtual Kullanici? Kullanici { get; set; }
    }
}
