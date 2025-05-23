using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Sepet
{
    public int SepetId { get; set; }

    public int KullaniciId { get; set; }

    public int EtkinlikId { get; set; }

    public int BiletSayisi { get; set; }

    public DateTime EklenmeTarihi { get; set; }

    public virtual Etkinlikler Etkinlik { get; set; }

    public virtual Kullanicilar Kullanici { get; set; }
}
