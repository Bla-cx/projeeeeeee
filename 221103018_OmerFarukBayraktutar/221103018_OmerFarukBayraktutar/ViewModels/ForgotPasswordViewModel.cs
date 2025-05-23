using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;
    }
}
