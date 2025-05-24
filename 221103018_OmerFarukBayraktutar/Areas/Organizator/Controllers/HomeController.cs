using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Organizator.Controllers
{    [Area("Organizator")]
    [Authorize(Roles = "Organizator")]
    public class HomeController : Controller
    {
        private readonly EtkinliklerDbContext _context;

        public HomeController(EtkinliklerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            
            var organizatorId = int.Parse(userId);
            
            var viewModel = new OrganizatorDashboardViewModel
            {
                ToplamEtkinlikSayisi = await _context.Etkinlikler.CountAsync(e => e.OrganizatorId == organizatorId),
                AktifEtkinlikSayisi = await _context.Etkinlikler.CountAsync(e => e.OrganizatorId == organizatorId && e.AktifMi && e.Tarih >= DateTime.Now),
                ToplamRezervasyonSayisi = await _context.Rezervasyonlar
                    .Include(r => r.Etkinlik)
                    .CountAsync(r => r.Etkinlik.OrganizatorId == organizatorId),
                BugunkuRezervasyonSayisi = await _context.Rezervasyonlar
                    .Include(r => r.Etkinlik)
                    .CountAsync(r => r.Etkinlik.OrganizatorId == organizatorId && r.RezervasyonTarihi.Date == DateTime.Now.Date),
                SonEtkinlikler = await _context.Etkinlikler
                    .Include(e => e.Kategori)
                    .Include(e => e.Sehir)
                    .Where(e => e.OrganizatorId == organizatorId)
                    .OrderByDescending(e => e.OlusturulmaTarihi)
                    .Take(5)
                    .ToListAsync(),
                SonRezervasyonlar = await _context.Rezervasyonlar
                    .Include(r => r.Etkinlik)
                    .Include(r => r.Kullanici)
                    .Where(r => r.Etkinlik.OrganizatorId == organizatorId)
                    .OrderByDescending(r => r.RezervasyonTarihi)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }    public class OrganizatorDashboardViewModel
    {
        public int ToplamEtkinlikSayisi { get; set; }
        public int AktifEtkinlikSayisi { get; set; }
        public int ToplamRezervasyonSayisi { get; set; }
        public int BugunkuRezervasyonSayisi { get; set; }
        public List<Etkinlik> SonEtkinlikler { get; set; } = new List<Etkinlik>();
        public List<Rezervasyon> SonRezervasyonlar { get; set; } = new List<Rezervasyon>();
    }
}
