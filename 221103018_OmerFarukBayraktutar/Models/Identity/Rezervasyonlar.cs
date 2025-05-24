using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Rezervasyonlar
{
    public int RezervasyonId { get; set; }

    public int EtkinlikId { get; set; }

    public int KullaniciId { get; set; }

    public int BiletSayisi { get; set; }

    public decimal ToplamFiyat { get; set; }

    public DateTime RezervasyonTarihi { get; set; }

    public string OdemeDurumu { get; set; }

    public string BarkodNo { get; set; }

    public virtual Etkinlikler Etkinlik { get; set; }

    public virtual Kullanicilar Kullanici { get; set; }
}
