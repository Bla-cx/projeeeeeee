using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RezervasyonController : Controller
    {
        private readonly EtkinliklerDbContext _context;

        public RezervasyonController(EtkinliklerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rezervasyonlar = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .Include(r => r.Kullanici)
                .OrderByDescending(r => r.RezervasyonTarihi)
                .ToListAsync();

            return View(rezervasyonlar);
        }

        public async Task<IActionResult> Details(int id)
        {
            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.RezervasyonId == id);

            if (rezervasyon == null)
            {
                return NotFound();
            }

            return View(rezervasyon);
        }

        [HttpPost]
        public async Task<IActionResult> Iptal(int id)
        {
            var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
            if (rezervasyon == null)
            {
                return NotFound();
            }

            var etkinlik = await _context.Etkinlikler.FindAsync(rezervasyon.EtkinlikId);
            if (etkinlik != null && !rezervasyon.Iptal)
            {
                // Kapasite güncelleme
                etkinlik.KalanKapasite += rezervasyon.BiletSayisi;
                rezervasyon.Iptal = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rezervasyon başarıyla iptal edildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rezervasyon zaten iptal edilmiş veya güncelleme yapılamadı.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Kullanildi(int id)
        {
            var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
            if (rezervasyon == null)
            {
                return NotFound();
            }

            if (!rezervasyon.Iptal && !rezervasyon.Kullanildi)
            {
                rezervasyon.Kullanildi = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rezervasyon kullanıldı olarak işaretlendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rezervasyon iptal edilmiş veya zaten kullanılmış.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.RezervasyonId == id);

            if (rezervasyon == null)
            {
                return NotFound();
            }

            return View(rezervasyon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervasyon = await _context.Rezervasyonlar.FindAsync(id);
            if (rezervasyon == null)
            {
                return NotFound();
            }

            var etkinlik = await _context.Etkinlikler.FindAsync(rezervasyon.EtkinlikId);
            if (etkinlik != null && !rezervasyon.Iptal)
            {
                // Kapasite güncelleme
                etkinlik.KalanKapasite += rezervasyon.BiletSayisi;
            }

            _context.Rezervasyonlar.Remove(rezervasyon);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rezervasyon başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
} 