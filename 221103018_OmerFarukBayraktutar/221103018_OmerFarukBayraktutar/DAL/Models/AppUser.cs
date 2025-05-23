using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    // IdentityUser'dan IdentityUser<int>'e değiştiriyoruz
    public class AppUser : IdentityUser<int>
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string Soyad { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
        public string Telefon { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? ProfilResimYolu { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Etkinlik> Etkinlikler { get; set; } = new List<Etkinlik>();
        public ICollection<Rezervasyon> Rezervasyonlar { get; set; } = new List<Rezervasyon>();
        public ICollection<Sepet> SepetOgeleri { get; set; } = new List<Sepet>();
        public ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();
        public ICollection<Log> Loglar { get; set; } = new List<Log>();
    }
}
