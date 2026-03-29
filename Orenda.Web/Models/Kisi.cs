using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Kisiler", Schema = "gayemkaratas_OrendaAdmin")]
public class Kisi
{
    [Key]
    public int CalisanID { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string KullaniciAdi { get; set; } = string.Empty;
    public string? RfidID { get; set; }
}