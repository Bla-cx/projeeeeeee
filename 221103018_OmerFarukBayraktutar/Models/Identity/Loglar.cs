using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Loglar
{
    public int LogId { get; set; }

    public int? KullaniciId { get; set; }

    public string IslemTipi { get; set; }

    public string Aciklama { get; set; }

    public string Ip { get; set; }

    public DateTime Tarih { get; set; }

    public virtual Kullanicilar Kullanici { get; set; }
}
