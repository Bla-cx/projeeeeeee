using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Organizator.Controllers
{
    [Area("Organizator")]
    [Authorize(Roles = "Organizer")]
    public class EtkinlikController : Controller
    {
        private readonly EtkinliklerDbContext _context;
        private readonly IEtkinlikService _etkinlikService;
        private readonly IFileService _fileService;

        public EtkinlikController(
            EtkinliklerDbContext context,
            IEtkinlikService etkinlikService,
            IFileService fileService)
        {
            _context = context;
            _etkinlikService = etkinlikService;
            _fileService = fileService;
        }

        // Organizatörün kendi etkinliklerini listeler
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var organizatorId = int.Parse(userId);
            var etkinlikler = await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Where(e => e.OrganizatorId == organizatorId)
                .OrderByDescending(e => e.OlusturulmaTarihi)
                .ToListAsync();

            return View(etkinlikler);
        }

        // Yeni etkinlik ekleme sayfası
        public async Task<IActionResult> Ekle()
        {
            var viewModel = new EtkinlikEkleViewModel
            {
                Kategoriler = await GetKategorilerSelectList(),
                Sehirler = await GetSehirlerSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(EtkinlikEkleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Kategoriler = await GetKategorilerSelectList();
                model.Sehirler = await GetSehirlerSelectList();
                return View(model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var organizatorId = int.Parse(userId);
            string resimYolu = "/img/defaults/default-event.svg"; // Varsayılan resim

            if (model.Resim != null)
            {
                resimYolu = await _fileService.SaveImageAsync(model.Resim, "etkinlikler");
            }

            var etkinlik = new Etkinlik
            {
                EtkinlikAdi = model.EtkinlikAdi,
                Aciklama = model.Aciklama,
                Tarih = model.Tarih,
                BaslangicSaati = model.BaslangicSaati,
                BitisSaati = model.BitisSaati,
                KategoriId = model.KategoriId,
                SehirId = model.SehirId,
                Adres = model.Adres,
                ResimYolu = resimYolu,
                OrganizatorId = organizatorId,
                BiletFiyati = model.BiletFiyati,
                ToplamKapasite = model.Kapasite,
                KalanKapasite = model.Kapasite,
                OlusturulmaTarihi = DateTime.Now,
                AktifMi = model.AktifMi
            };

            _context.Etkinlikler.Add(etkinlik);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinlik başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // Etkinlik düzenleme sayfası
        public async Task<IActionResult> Duzenle(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var organizatorId = int.Parse(userId);
            var etkinlik = await _context.Etkinlikler.FindAsync(id);

            if (etkinlik == null)
            {
                return NotFound();
            }

            // Sadece kendi etkinliklerini düzenleyebilir
            if (etkinlik.OrganizatorId != organizatorId)
            {
                return Forbid();
            }

            var viewModel = new EtkinlikDuzenleViewModel
            {
                EtkinlikId = etkinlik.EtkinlikId,
                EtkinlikAdi = etkinlik.EtkinlikAdi,
                Aciklama = etkinlik.Aciklama,
                Tarih = etkinlik.Tarih,
                BaslangicSaati = etkinlik.BaslangicSaati,
                BitisSaati = etkinlik.BitisSaati,
                KategoriId = etkinlik.KategoriId,
                SehirId = etkinlik.SehirId,
                Adres = etkinlik.Adres,
                MevcutResimYolu = etkinlik.ResimYolu,
                BiletFiyati = etkinlik.BiletFiyati,
                Kapasite = etkinlik.ToplamKapasite,
                AktifMi = etkinlik.AktifMi,
                Kategoriler = await GetKategorilerSelectList(),
                Sehirler = await GetSehirlerSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(EtkinlikDuzenleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Kategoriler = await GetKategorilerSelectList();
                model.Sehirler = await GetSehirlerSelectList();
                return View(model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var organizatorId = int.Parse(userId);
            var etkinlik = await _context.Etkinlikler.FindAsync(model.EtkinlikId);

            if (etkinlik == null)
            {
                return NotFound();
            }

            if (etkinlik.OrganizatorId != organizatorId)
            {
                return Forbid();
            }

            string resimYolu = etkinlik.ResimYolu;

            if (model.Resim != null)
            {
                resimYolu = await _fileService.SaveImageAsync(model.Resim, "etkinlikler");
            }

            // Etkinliği güncelle
            etkinlik.EtkinlikAdi = model.EtkinlikAdi;
            etkinlik.Aciklama = model.Aciklama;
            etkinlik.Tarih = model.Tarih;
            etkinlik.BaslangicSaati = model.BaslangicSaati;
            etkinlik.BitisSaati = model.BitisSaati;
            etkinlik.KategoriId = model.KategoriId;
            etkinlik.SehirId = model.SehirId;
            etkinlik.Adres = model.Adres;
            etkinlik.ResimYolu = resimYolu;
            etkinlik.BiletFiyati = model.BiletFiyati;
            
            // Kapasite kontrolü - mevcut rezervasyonlar var mı?
            if (model.Kapasite < etkinlik.ToplamKapasite - etkinlik.KalanKapasite)
            {
                ModelState.AddModelError("Kapasite", "Yeni kapasite, mevcut rezervasyon sayısından az olamaz.");
                model.Kategoriler = await GetKategorilerSelectList();
                model.Sehirler = await GetSehirlerSelectList();
                return View(model);
            }

            // Kapasite değişikliği varsa, kalan kapasiteyi güncelle
            if (model.Kapasite != etkinlik.ToplamKapasite)
            {
                var rezervasyonSayisi = etkinlik.ToplamKapasite - etkinlik.KalanKapasite;
                etkinlik.ToplamKapasite = model.Kapasite;
                etkinlik.KalanKapasite = model.Kapasite - rezervasyonSayisi;
            }

            etkinlik.AktifMi = model.AktifMi;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinlik başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // Etkinlik detay sayfası
        public async Task<IActionResult> Detay(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var organizatorId = int.Parse(userId);
            var etkinlik = await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .FirstOrDefaultAsync(e => e.EtkinlikId == id);

            if (etkinlik == null)
            {
                return NotFound();
            }

            if (etkinlik.OrganizatorId != organizatorId)
            {
                return Forbid();
            }

            // Etkinliğe ait rezervasyonları getir
            var rezervasyonlar = await _context.Rezervasyonlar
                .Include(r => r.Kullanici)
                .Where(r => r.EtkinlikId == id)
                .OrderByDescending(r => r.RezervasyonTarihi)
                .ToListAsync();

            var viewModel = new EtkinlikDetayViewModel
            {
                Etkinlik = etkinlik,
                Rezervasyonlar = rezervasyonlar
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var organizatorId = int.Parse(userId);
            var etkinlik = await _context.Etkinlikler.FindAsync(id);

            if (etkinlik == null)
            {
                return NotFound();
            }

            if (etkinlik.OrganizatorId != organizatorId)
            {
                return Forbid();
            }

            // Etkinliğe ait rezervasyon kontrolü
            var rezervasyonSayisi = await _context.Rezervasyonlar.CountAsync(r => r.EtkinlikId == id);
            if (rezervasyonSayisi > 0)
            {
                TempData["ErrorMessage"] = "Bu etkinliğe ait rezervasyonlar olduğu için silinemez. Bunun yerine etkinliği pasif yapabilirsiniz.";
                return RedirectToAction(nameof(Index));
            }

            _context.Etkinlikler.Remove(etkinlik);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Etkinlik başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        // Helper metotlar
        private async Task<List<SelectListItem>> GetKategorilerSelectList()
        {
            var kategoriler = await _context.Kategoriler.OrderBy(k => k.KategoriAdi).ToListAsync();
            return kategoriler.Select(k => new SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAdi
            }).ToList();
        }

        private async Task<List<SelectListItem>> GetSehirlerSelectList()
        {
            var sehirler = await _context.Sehirler.OrderBy(s => s.SehirAdi).ToListAsync();
            return sehirler.Select(s => new SelectListItem
            {
                Value = s.SehirId.ToString(),
                Text = s.SehirAdi
            }).ToList();
        }
    }

    public class EtkinlikDetayViewModel
    {
        public Etkinlik Etkinlik { get; set; }
        public List<Rezervasyon> Rezervasyonlar { get; set; }
    }
} 