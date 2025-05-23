using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Etkinlikler
{
    public int EtkinlikId { get; set; }

    public string EtkinlikAdi { get; set; }

    public string Aciklama { get; set; }

    public DateTime Tarih { get; set; }

    public TimeOnly? BaslangicSaati { get; set; }

    public TimeOnly? BitisSaati { get; set; }

    public int KategoriId { get; set; }

    public int SehirId { get; set; }

    public string Adres { get; set; }

    public string ResimYolu { get; set; }

    public int OrganizatorId { get; set; }

    public decimal BiletFiyati { get; set; }

    public int ToplamKapasite { get; set; }

    public int KalanKapasite { get; set; }

    public DateTime OlusturulmaTarihi { get; set; }

    public bool AktifMi { get; set; }

    public virtual Kategoriler Kategori { get; set; }

    public virtual Kullanicilar Organizator { get; set; }

    public virtual ICollection<Rezervasyonlar> Rezervasyonlars { get; set; } = new List<Rezervasyonlar>();

    public virtual Sehirler Sehir { get; set; }

    public virtual ICollection<Sepet> Sepets { get; set; } = new List<Sepet>();
}
