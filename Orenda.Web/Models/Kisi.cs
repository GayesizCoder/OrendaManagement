using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Kisiler", Schema = "gayemkaratas_OrendaAdmin")]
public class Kisi
{
    [Key]
    public int CalisanID { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string KullaniciAdi { get; set; }
    public string? RfidID { get; set; }
}