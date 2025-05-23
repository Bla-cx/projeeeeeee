using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.Models.Identity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [Display(Name = "Adınız")]
        public string? Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [Display(Name = "Soyadınız")]
        public string? Soyad { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta Adresiniz")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [Display(Name = "Telefon Numarası")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profil Resmi")]
        public string? ProfilResimYolu { get; set; }

        [Display(Name = "Yeni Profil Resmi")]
        public IFormFile? NewProfileImage { get; set; }

        // Kullanıcının rezervasyonları
        public IEnumerable<Rezervasyon>? Rezervasyonlar { get; set; }
        
        // Profil resmi için güvenli yol alır
        public string GetSafeProfileImagePath()
        {
            return ProfilResimYolu ?? "/img/defaults/default-user.svg";
        }
    }
}
