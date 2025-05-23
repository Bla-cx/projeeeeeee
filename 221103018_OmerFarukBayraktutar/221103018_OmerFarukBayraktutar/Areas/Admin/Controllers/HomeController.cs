using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly EtkinliklerDbContext _context;

        public HomeController(EtkinliklerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikleri topla
            var viewModel = new AdminDashboardViewModel
            {
                ToplamKullaniciSayisi = await _context.Users.CountAsync(),
                ToplamEtkinlikSayisi = await _context.Etkinlikler.CountAsync(),
                ToplamRezervasyonSayisi = await _context.Rezervasyonlar.CountAsync(),
                AktifEtkinlikSayisi = await _context.Etkinlikler.CountAsync(e => e.AktifMi && e.Tarih >= DateTime.Now),
                BugunkuRezervasyonSayisi = await _context.Rezervasyonlar.CountAsync(r => r.RezervasyonTarihi.Date == DateTime.Now.Date),
                SonEtkinlikler = await _context.Etkinlikler
                    .Include(e => e.Kategori)
                    .Include(e => e.Sehir)
                    .OrderByDescending(e => e.OlusturulmaTarihi)
                    .Take(5)
                    .ToListAsync(),
                SonRezervasyonlar = await _context.Rezervasyonlar
                    .Include(r => r.Etkinlik)
                    .Include(r => r.Kullanici)
                    .OrderByDescending(r => r.RezervasyonTarihi)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }

    public class AdminDashboardViewModel
    {
        public int ToplamKullaniciSayisi { get; set; }
        public int ToplamEtkinlikSayisi { get; set; }
        public int ToplamRezervasyonSayisi { get; set; }
        public int AktifEtkinlikSayisi { get; set; }
        public int BugunkuRezervasyonSayisi { get; set; }
        public List<Etkinlik> SonEtkinlikler { get; set; }
        public List<Rezervasyon> SonRezervasyonlar { get; set; }
    }
}
