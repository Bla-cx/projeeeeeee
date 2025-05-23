using System;
using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "Kart numarası zorunludur.")]
        [Display(Name = "Kart Numarası")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Kart numarası 16 haneli olmalıdır.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Kart sahibi adı zorunludur.")]
        [Display(Name = "Kart Sahibi")]
        public string CardHolder { get; set; }

        [Required(ErrorMessage = "Son kullanma ayı zorunludur.")]
        [Display(Name = "Son Kullanma Ayı")]
        [Range(1, 12, ErrorMessage = "Geçerli bir ay giriniz (1-12).")]
        public int ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Son kullanma yılı zorunludur.")]
        [Display(Name = "Son Kullanma Yılı")]
        [Range(2023, 2050, ErrorMessage = "Geçerli bir yıl giriniz.")]
        public int ExpiryYear { get; set; }

        [Required(ErrorMessage = "CVV kodu zorunludur.")]
        [Display(Name = "Güvenlik Kodu (CVV)")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV kodu 3 haneli olmalıdır.")]
        public string CVV { get; set; }

        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }
    }
}
