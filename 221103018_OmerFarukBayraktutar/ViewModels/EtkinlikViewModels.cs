using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class EtkinlikViewModel
    {
        public int EtkinlikId { get; set; }

        [Required(ErrorMessage = "Etkinlik adı zorunludur.")]
        [Display(Name = "Etkinlik Adı")]
        public string EtkinlikAdi { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan? BaslangicSaati { get; set; }

        [Required(ErrorMessage = "Bitiş saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan? BitisSaati { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int KategoriId { get; set; }

        [Required(ErrorMessage = "Şehir seçimi zorunludur.")]
        [Display(Name = "Şehir")]
        public int SehirId { get; set; }

        [Required(ErrorMessage = "Adres zorunludur.")]
        [Display(Name = "Adres")]
        public string Adres { get; set; }

        [Display(Name = "Resim")]
        public IFormFile Resim { get; set; }

        [Required(ErrorMessage = "Bilet fiyatı zorunludur.")]
        [Display(Name = "Bilet Fiyatı (₺)")]
        [Range(0, 10000, ErrorMessage = "Bilet fiyatı 0 ile 10.000 TL arasında olmalıdır.")]
        public decimal BiletFiyati { get; set; }

        [Required(ErrorMessage = "Kapasite zorunludur.")]
        [Display(Name = "Kapasite")]
        [Range(1, 100000, ErrorMessage = "Kapasite en az 1 olmalıdır.")]
        public int Kapasite { get; set; }

        [Display(Name = "Aktif")]
        public bool AktifMi { get; set; } = true;

        [Display(Name = "Organizatör")]
        public int OrganizatorId { get; set; }

        public string KategoriAdi { get; set; }
        public string SehirAdi { get; set; }
        public string ResimYolu { get; set; }
        public string OrganizatorAdi { get; set; }
        public int KalanKapasite { get; set; }
    }

    public class EtkinlikEkleViewModel : EtkinlikViewModel
    {
        public List<SelectListItem> Kategoriler { get; set; }
        public List<SelectListItem> Sehirler { get; set; }
        public List<SelectListItem> OrganizatorList { get; set; }
    }

    public class EtkinlikDuzenleViewModel : EtkinlikEkleViewModel
    {
        public string MevcutResimYolu { get; set; }
    }

    public class AdminEtkinlikListeViewModel
    {
        public List<EtkinlikViewModel> Etkinlikler { get; set; }
        public int? KategoriId { get; set; }
        public int? SehirId { get; set; }
        public string AramaMetni { get; set; }
        public DateTime? Tarih { get; set; }
        public List<SelectListItem> Kategoriler { get; set; }
        public List<SelectListItem> Sehirler { get; set; }
    }

    public class AdminEtkinlikDetayViewModel
    {
        public EtkinlikViewModel Etkinlik { get; set; }
        public List<Rezervasyon> Rezervasyonlar { get; set; }
        public int BiletSayisi { get; set; } = 1;
    }
} 