using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _221103018_OmerFarukBayraktutar.Controllers
{
    public class EtkinlikController : Controller
    {
        private readonly IEtkinlikService _etkinlikService;
        private readonly EtkinliklerDbContext _context;
        private readonly ILogger<EtkinlikController> _logger;

        public EtkinlikController(
            IEtkinlikService etkinlikService,
            EtkinliklerDbContext context,
            ILogger<EtkinlikController> logger)
        {
            _etkinlikService = etkinlikService;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele(string? q, int? kategoriId, int? sehirId, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var etkinlikler = await _etkinlikService.SearchAsync(q, kategoriId, sehirId, baslangicTarihi, bitisTarihi);
            var kategoriler = await _context.Kategoriler.ToListAsync();
            var sehirler = await _context.Sehirler.ToListAsync();

            var viewModel = new EtkinlikListeViewModel
            {
                Etkinlikler = etkinlikler,
                Kategoriler = kategoriler,
                Sehirler = sehirler,
                Arama = q,
                KategoriId = kategoriId,
                SehirId = sehirId,
                BaslangicTarihi = baslangicTarihi,
                BitisTarihi = bitisTarihi
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var etkinlik = await _etkinlikService.GetByIdAsync(id);
            if (etkinlik == null)
            {
                return NotFound();
            }

            var benzerEtkinlikler = await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Where(e => e.KategoriId == etkinlik.KategoriId && 
                            e.EtkinlikId != etkinlik.EtkinlikId && 
                            e.AktifMi && 
                            e.Tarih > DateTime.Now)
                .Take(3)
                .ToListAsync();

            var viewModel = new EtkinlikDetayViewModel
            {
                Etkinlik = etkinlik,
                BenzerEtkinlikler = benzerEtkinlikler
            };

            return View(viewModel);
        }
    }
}
