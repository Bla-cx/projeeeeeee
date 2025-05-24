using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Telefon { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? ProfilResimYolu { get; set; }
    }
}
