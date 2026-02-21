using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ToDo", Schema = "gayemkaratas_OrendaAdmin")]
public class ToDo
{
    [Key]
    public int GorevNo { get; set; }

    // Kisiler tablosundaki CalisanID ile eşleşen Foreign Key
    public int AtananCalisanID { get; set; }

    public string Baslik { get; set; }
    public string? Aciklama { get; set; }
    public string Durum { get; set; } = "Yapilacak";
    public DateTime? BitisTarihi { get; set; }
}