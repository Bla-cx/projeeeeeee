using _221103018_OmerFarukBayraktutar.DAL.Models;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class EtkinlikDetayViewModel
    {
        public Etkinlik? Etkinlik { get; set; }
        public List<Etkinlik> BenzerEtkinlikler { get; set; } = new List<Etkinlik>();
    }
} 