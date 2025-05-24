using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.DAL.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public string RolAdi { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Kullanici> Kullanicilar { get; set; } = new List<Kullanici>();
    }
}
