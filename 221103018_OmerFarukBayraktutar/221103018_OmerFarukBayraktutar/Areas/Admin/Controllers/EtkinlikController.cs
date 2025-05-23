using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using _221103018_OmerFarukBayraktutar.BLL.Interfaces;

namespace _221103018_OmerFarukBayraktutar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EtkinlikController : Controller
    {
        private readonly EtkinliklerDbContext _context;
        private readonly IFileService _fileService;

        public EtkinlikController(EtkinliklerDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var etkinlikler = await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Include(e => e.Organizator)
                .OrderByDescending(e => e.OlusturulmaTarihi)
                .ToListAsync();

            return View(etkinlikler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var etkinlik = await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Include(e => e.Organizator)
                .FirstOrDefaultAsync(e => e.EtkinlikId == id);

            if (etkinlik == null)
            {
                return NotFound();
            }

            var rezervasyonlar = await _context.Rezervasyonlar
                .Include(r => r.Kullanici)
                .Where(r => r.EtkinlikId == id)
                .OrderByDescending(r => r.RezervasyonTarihi)
                .ToListAsync();

            var viewModel = new AdminEtkinlikDetayViewModel
            {
                Etkinlik = etkinlik,
                Rezervasyonlar = rezervasyonlar
            };

            return View(viewModel);
        }

        // GET: Admin/Etkinlik/Ekle
        public async Task<IActionResult> Ekle()
        {
            var viewModel = new EtkinlikEkleViewModel
            {
                Kategoriler = await GetKategorilerSelectList(),
                Sehirler = await GetSehirlerSelectList(),
                OrganizatorList = await GetOrganizatorSelectList()
            };

            return View(viewModel);
        }

        // POST: Admin/Etkinlik/Ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(EtkinlikEkleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Kategoriler = await GetKategorilerSelectList();
                model.Sehirler = await GetSehirlerSelectList();
                model.OrganizatorList = await GetOrganizatorSelectList();
                return View(model);
            }

            string resimYolu = "/img/defaults/default-event.svg"; // Varsayılan resim

            if (model.Resim != null && model.Resim.Length > 5 * 1024 * 1024) // 5MB
            {
                ModelState.AddModelError("Resim", "Dosya boyutu 5MB'ı geçemez.");
                return View(model);
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
                OrganizatorId = model.OrganizatorId,
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

        // GET: Admin/Etkinlik/Duzenle/5
        public async Task<IActionResult> Duzenle(int id)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(id);

            if (etkinlik == null)
            {
                return NotFound();
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
                OrganizatorId = etkinlik.OrganizatorId,
                Kategoriler = await GetKategorilerSelectList(),
                Sehirler = await GetSehirlerSelectList(),
                OrganizatorList = await GetOrganizatorSelectList()
            };

            return View(viewModel);
        }

        // POST: Admin/Etkinlik/Duzenle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(EtkinlikDuzenleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Kategoriler = await GetKategorilerSelectList();
                model.Sehirler = await GetSehirlerSelectList();
                model.OrganizatorList = await GetOrganizatorSelectList();
                return View(model);
            }

            var etkinlik = await _context.Etkinlikler.FindAsync(model.EtkinlikId);

            if (etkinlik == null)
            {
                return NotFound();
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
            etkinlik.OrganizatorId = model.OrganizatorId;
            
            // Kapasite kontrolü - mevcut rezervasyonlar var mı?
            if (model.Kapasite < etkinlik.ToplamKapasite - etkinlik.KalanKapasite)
            {
                ModelState.AddModelError("Kapasite", "Yeni kapasite, mevcut rezervasyon sayısından az olamaz.");
                model.Kategoriler = await GetKategorilerSelectList();
                model.Sehirler = await GetSehirlerSelectList();
                model.OrganizatorList = await GetOrganizatorSelectList();
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

        [HttpPost]
        public async Task<IActionResult> DurumDegistir(int id)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null)
            {
                return NotFound();
            }

            etkinlik.AktifMi = !etkinlik.AktifMi;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Etkinlik durumu başarıyla {(etkinlik.AktifMi ? "aktif" : "pasif")} yapıldı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Sil(int id)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik == null)
            {
                return NotFound();
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

        // Yardımcı metotlar
        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetKategorilerSelectList()
        {
            var kategoriler = await _context.Kategoriler.OrderBy(k => k.KategoriAdi).ToListAsync();
            return kategoriler.Select(k => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAdi
            }).ToList();
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetSehirlerSelectList()
        {
            var sehirler = await _context.Sehirler.OrderBy(s => s.SehirAdi).ToListAsync();
            return sehirler.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.SehirId.ToString(),
                Text = s.SehirAdi
            }).ToList();
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetOrganizatorSelectList()
        {
            var users = await _context.Users
                .Where(u => u.UserName != "admin") // Exclude admin
                .OrderBy(u => u.Ad)
                .ToListAsync();

            var organizatorList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            
            foreach (var user in users)
            {
                // Get user roles from UserRoles table
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .ToListAsync();

                // Get role IDs
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                // Get role names
                var roles = await _context.Roles
                    .Where(r => roleIds.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToListAsync();

                // Add user to list if they have Organizer role
                if (roles.Contains("Organizer"))
                {
                    organizatorList.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = user.Id.ToString(),
                        Text = $"{user.Ad} {user.Soyad} ({user.Email})"
                    });
                }
            }
            
            return organizatorList;
        }
    }

    public class AdminEtkinlikDetayViewModel
    {
        public Etkinlik Etkinlik { get; set; }
        public List<Rezervasyon> Rezervasyonlar { get; set; }
    }
} 