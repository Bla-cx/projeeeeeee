using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Sehir
    {
        [Key]
        public int SehirId { get; set; }

        [Required(ErrorMessage = "Şehir adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Şehir adı en fazla 100 karakter olabilir.")]
        public string SehirAdi { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Etkinlik> Etkinlikler { get; set; } = new List<Etkinlik>();
    }
}
