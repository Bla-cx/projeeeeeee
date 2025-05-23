using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Rezervasyon
    {
        [Key]
        public int RezervasyonId { get; set; }

        [Required(ErrorMessage = "Etkinlik seçimi zorunludur.")]
        public int EtkinlikId { get; set; }

        [ForeignKey("EtkinlikId")]
        public Etkinlik? Etkinlik { get; set; }

        [Required(ErrorMessage = "Kullanıcı bilgisi zorunludur.")]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public AppUser? Kullanici { get; set; }

        [Required(ErrorMessage = "Bilet sayısı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Bilet sayısı en az 1 olmalıdır.")]
        public int BiletSayisi { get; set; }

        [Required(ErrorMessage = "Toplam tutar zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar sıfır veya pozitif olmalıdır.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ToplamTutar { get; set; }

        public DateTime RezervasyonTarihi { get; set; } = DateTime.Now;

        public bool OdemeDurumu { get; set; } = false;

        [StringLength(50)]
        public string? BarkodNo { get; set; }
        
        public bool Iptal { get; set; } = false;
        
        public bool Kullanildi { get; set; } = false;
    }
}
