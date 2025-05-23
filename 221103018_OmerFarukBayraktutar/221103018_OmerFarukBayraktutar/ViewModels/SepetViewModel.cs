using _221103018_OmerFarukBayraktutar.DAL.Models;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class SepetViewModel
    {
        public IEnumerable<Sepet> SepetOgeleri { get; set; } = new List<Sepet>();
        public decimal ToplamTutar { get; set; }
    }
}
