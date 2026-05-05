using System.ComponentModel.DataAnnotations;

namespace Orenda.Web.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Soyad { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Eposta { get; set; }

        public string? Telefon { get; set; }

        public string? MevcutSifre { get; set; }

        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır")]
        public string? YeniSifre { get; set; }

        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor")]
        public string? YeniSifreTekrar { get; set; }

        public string GlobalID { get; set; } = string.Empty;
    }
}
