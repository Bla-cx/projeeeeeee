using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SehirController : Controller
    {
        private readonly EtkinliklerDbContext _context;

        public SehirController(EtkinliklerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sehirler = await _context.Sehirler.ToListAsync();
            return View(sehirler);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sehir sehir)
        {
            if (ModelState.IsValid)
            {
                var existingSehir = await _context.Sehirler
                    .FirstOrDefaultAsync(s => s.SehirAdi == sehir.SehirAdi);

                if (existingSehir != null)
                {
                    ModelState.AddModelError("SehirAdi", "Bu isimde bir şehir zaten mevcut.");
                    return View(sehir);
                }

                _context.Add(sehir);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Şehir başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(sehir);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sehir = await _context.Sehirler.FindAsync(id);
            if (sehir == null)
            {
                return NotFound();
            }
            return View(sehir);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sehir sehir)
        {
            if (id != sehir.SehirId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingSehir = await _context.Sehirler
                    .FirstOrDefaultAsync(s => s.SehirAdi == sehir.SehirAdi && s.SehirId != id);

                if (existingSehir != null)
                {
                    ModelState.AddModelError("SehirAdi", "Bu isimde bir şehir zaten mevcut.");
                    return View(sehir);
                }

                try
                {
                    _context.Update(sehir);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Şehir başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SehirExists(sehir.SehirId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sehir);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var sehir = await _context.Sehirler.FindAsync(id);
            if (sehir == null)
            {
                return NotFound();
            }

            return View(sehir);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sehir = await _context.Sehirler.FindAsync(id);
            if (sehir == null)
            {
                return NotFound();
            }

            // İlişkili etkinlikleri kontrol et
            var hasEvents = await _context.Etkinlikler.AnyAsync(e => e.SehirId == id);
            if (hasEvents)
            {
                TempData["ErrorMessage"] = "Bu şehire ait etkinlikler olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _context.Sehirler.Remove(sehir);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Şehir başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private bool SehirExists(int id)
        {
            return _context.Sehirler.Any(e => e.SehirId == id);
        }
    }
}
