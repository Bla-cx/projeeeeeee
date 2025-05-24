using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Sepet
    {
        [Key]
        public int SepetId { get; set; }

        [Required(ErrorMessage = "Kullanıcı bilgisi zorunludur.")]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public AppUser? Kullanici { get; set; }

        [Required(ErrorMessage = "Etkinlik seçimi zorunludur.")]
        public int EtkinlikId { get; set; }

        [ForeignKey("EtkinlikId")]
        public Etkinlik? Etkinlik { get; set; }

        [Required(ErrorMessage = "Bilet sayısı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Bilet sayısı en az 1 olmalıdır.")]
        public int BiletSayisi { get; set; } = 1;

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}
