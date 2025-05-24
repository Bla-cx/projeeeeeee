using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class EtkinlikInt
    {
        [Key]
        public int EtkinlikId { get; set; }

        [Required(ErrorMessage = "Etkinlik adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Etkinlik adı en fazla 200 karakter olabilir.")]
        public string EtkinlikAdi { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        public DateTime Tarih { get; set; }

        public TimeSpan? BaslangicSaati { get; set; }

        public TimeSpan? BitisSaati { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int KategoriId { get; set; }

        [ForeignKey("KategoriId")]
        public Kategori? Kategori { get; set; }

        [Required(ErrorMessage = "Şehir seçimi zorunludur.")]
        public int SehirId { get; set; }

        [ForeignKey("SehirId")]
        public Sehir? Sehir { get; set; }

        [Required(ErrorMessage = "Adres zorunludur.")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        public string Adres { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ResimYolu { get; set; }

        // OrganizatorId artık int türünde
        [Required(ErrorMessage = "Organizatör bilgisi zorunludur.")]
        public int OrganizatorId { get; set; }

        [ForeignKey("OrganizatorId")]
        public AppUser? Organizator { get; set; }

        [Required(ErrorMessage = "Kapasite zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Kapasite en az 1 olmalıdır.")]
        public int Kapasite { get; set; }

        [Required(ErrorMessage = "Kalan kapasite zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Kalan kapasite sıfır veya pozitif olmalıdır.")]
        public int KalanKapasite { get; set; }

        [Required(ErrorMessage = "Bilet fiyatı zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Bilet fiyatı sıfır veya pozitif olmalıdır.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal BiletFiyati { get; set; }

        public bool AktifMi { get; set; } = true;

        // Navigation properties
        public ICollection<Rezervasyon> Rezervasyonlar { get; set; } = new List<Rezervasyon>();
        public ICollection<Sepet> SepetOgeleri { get; set; } = new List<Sepet>();
    }
}
