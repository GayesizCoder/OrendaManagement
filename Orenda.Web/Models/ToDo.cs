using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    // SQL'deki 'ToDo' tablosunu ve şemasını buraya bağladık
    [Table("ToDo", Schema = "gayemkaratas_OrendaAdmin")]
    public class ToDo
    {
        [Key]
        public int GorevNo { get; set; } // SQL'deki Primary Key
        public int AtananCalisanID { get; set; }
        public string Baslik { get; set; }
        public string? Aciklama { get; set; }
        public string Durum { get; set; } // Default: 'Yapilacak'
        public DateTime? BitisTarihi { get; set; }
    }
}