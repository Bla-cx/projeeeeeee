using _221103018_OmerFarukBayraktutar.DAL.Models;
using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class EtkinlikListeViewModel
    {
        public List<Etkinlik> Etkinlikler { get; set; } = new List<Etkinlik>();
        public List<Kategori> Kategoriler { get; set; } = new List<Kategori>();
        public List<Sehir> Sehirler { get; set; } = new List<Sehir>();
        public string? Arama { get; set; }
        public int? KategoriId { get; set; }
        public int? SehirId { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }
} 