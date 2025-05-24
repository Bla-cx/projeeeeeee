using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Bildirim
    {
        [Key]
        public int BildirimId { get; set; }

        [Required(ErrorMessage = "Kullanıcı bilgisi zorunludur.")]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public AppUser? Kullanici { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur.")]
        [StringLength(500, ErrorMessage = "Mesaj en fazla 500 karakter olabilir.")]
        public string Mesaj { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.Now;

        public bool Okundu { get; set; } = false;
    }
}
