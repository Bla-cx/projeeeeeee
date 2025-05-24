using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta adresi en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string SifreHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string Soyad { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
        public string? Telefon { get; set; }

        public bool EmailOnaylandi { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }

        public Guid EmailOnayKodu { get; set; } = Guid.NewGuid();

        // Navigation properties
        public ICollection<Etkinlik> Etkinlikler { get; set; } = new List<Etkinlik>();
        public ICollection<Rezervasyon> Rezervasyonlar { get; set; } = new List<Rezervasyon>();
        public ICollection<Sepet> SepetOgeleri { get; set; } = new List<Sepet>();
        public ICollection<Bildirim> Bildirimler { get; set; } = new List<Bildirim>();
        public ICollection<Log> Loglar { get; set; } = new List<Log>();
    }
}
