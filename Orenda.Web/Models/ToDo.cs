using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orenda.Web.Models
{
    [Table("ToDo", Schema = "gayemkaratas_OrendaAdmin")]
    public class ToDo
    {
        [Key]
        public int GorevNo { get; set; } // SQL'deki Primary Key

        public int? AtananCalisanID { get; set; } // Foreign Key

        [Required(ErrorMessage = "Başlık boş bırakılamaz")]
        [MaxLength(150)]
        public string Baslik { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        [MaxLength(50)]
        public string Durum { get; set; } = "Yapilacak"; // Varsayılan durum

        public DateTime? BitisTarihi { get; set; }
        
        public int? AtayanCalisanID { get; set; }
        
        public DateTime? BaslangicTarihi { get; set; }

        [MaxLength(50)]
        public string OnayDurumu { get; set; } = "Planlanıyor";

        [MaxLength(500)]
        public string? OnayNotu { get; set; }

        public int? TakimID { get; set; }

        // Navigation Properties
        [ForeignKey("AtananCalisanID")]
        public virtual Kullanici? AtananKisi { get; set; }

        [ForeignKey("TakimID")]
        public virtual Takim? Takim { get; set; }

        public virtual ICollection<GorevAdimi> Adimlar { get; set; } = new List<GorevAdimi>();

        [NotMapped]
        public double TamamlanmaOrani
        {
            get
            {
                if (Adimlar == null || !Adimlar.Any())
                {
                    return (Durum != null && (Durum.Contains("Tamamland") || Durum == "Bitti" || Durum == "Done")) ? 100 : 0;
                }

                double totalReallized = 0;
                double explicitPercentSum = 0;
                int defaultItemsCount = 0;

                foreach (var adim in Adimlar)
                {
                    if (adim.AgirlikYuzdesi.HasValue)
                    {
                        explicitPercentSum += adim.AgirlikYuzdesi.Value;
                        if (adim.TamamlandiMi)
                            totalReallized += adim.AgirlikYuzdesi.Value;
                    }
                    else
                    {
                        defaultItemsCount++;
                    }
                }

                if (defaultItemsCount > 0)
                {
                    double remainingPercent = Math.Max(0, 100 - explicitPercentSum);
                    double weightPerDefaultItem = remainingPercent / defaultItemsCount;

                    foreach (var adim in Adimlar)
                    {
                        if (!adim.AgirlikYuzdesi.HasValue && adim.TamamlandiMi)
                        {
                            totalReallized += weightPerDefaultItem;
                        }
                    }
                }

                return Math.Min(100, Math.Max(0, Math.Round(totalReallized, 2)));
            }
        }
    }
}