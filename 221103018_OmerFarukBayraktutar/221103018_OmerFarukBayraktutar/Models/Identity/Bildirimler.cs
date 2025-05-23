using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Bildirimler
{
    public int BildirimId { get; set; }

    public int KullaniciId { get; set; }

    public string Mesaj { get; set; }

    public DateTime Tarih { get; set; }

    public bool Okundu { get; set; }

    public virtual Kullanicilar Kullanici { get; set; }
}
