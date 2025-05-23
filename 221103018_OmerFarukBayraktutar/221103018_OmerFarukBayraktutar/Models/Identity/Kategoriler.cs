using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class Kategoriler
{
    public int KategoriId { get; set; }

    public string KategoriAdi { get; set; }

    public string Aciklama { get; set; }

    public virtual ICollection<Etkinlikler> Etkinliklers { get; set; } = new List<Etkinlikler>();
}
