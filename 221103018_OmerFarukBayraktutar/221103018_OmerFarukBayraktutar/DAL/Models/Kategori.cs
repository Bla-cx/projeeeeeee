using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Kategori
    {
        [Key]
        public int KategoriId { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        public string KategoriAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Aciklama { get; set; }

        // Navigation property
        public ICollection<Etkinlik> Etkinlikler { get; set; } = new List<Etkinlik>();
    }
}
