using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }

        public int? KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public AppUser? Kullanici { get; set; }

        [Required(ErrorMessage = "İşlem tipi zorunludur.")]
        [StringLength(50, ErrorMessage = "İşlem tipi en fazla 50 karakter olabilir.")]
        public string IslemTipi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Aciklama { get; set; }

        [StringLength(50)]
        public string? IP { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}
