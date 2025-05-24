using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Kullanicilar
{
    public int KullaniciId { get; set; }

    public string KullaniciAdi { get; set; }

    public string Email { get; set; }

    public string SifreHash { get; set; }

    public string Ad { get; set; }

    public string Soyad { get; set; }

    public string Telefon { get; set; }

    public bool EmailOnaylandi { get; set; }

    public DateTime KayitTarihi { get; set; }

    public int RolId { get; set; }

    public Guid? EmailOnayKodu { get; set; }

    public virtual ICollection<Bildirimler> Bildirimlers { get; set; } = new List<Bildirimler>();

    public virtual ICollection<Etkinlikler> Etkinliklers { get; set; } = new List<Etkinlikler>();

    public virtual ICollection<Loglar> Loglars { get; set; } = new List<Loglar>();

    public virtual ICollection<Rezervasyonlar> Rezervasyonlars { get; set; } = new List<Rezervasyonlar>();

    public virtual Roller Rol { get; set; }

    public virtual ICollection<Sepet> Sepets { get; set; } = new List<Sepet>();
}
