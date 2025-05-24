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
    public class KategoriController : Controller
    {
        private readonly EtkinliklerDbContext _context;

        public KategoriController(EtkinliklerDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kategoriler = await _context.Kategoriler.ToListAsync();
            return View(kategoriler);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Kategoriler
                    .FirstOrDefaultAsync(k => k.KategoriAdi == kategori.KategoriAdi);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("KategoriAdi", "Bu isimde bir kategori zaten mevcut.");
                    return View(kategori);
                }

                _context.Add(kategori);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }
            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Kategori kategori)
        {
            if (id != kategori.KategoriId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Kategoriler
                    .FirstOrDefaultAsync(k => k.KategoriAdi == kategori.KategoriAdi && k.KategoriId != id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("KategoriAdi", "Bu isimde bir kategori zaten mevcut.");
                    return View(kategori);
                }

                try
                {
                    _context.Update(kategori);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriExists(kategori.KategoriId))
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
            return View(kategori);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }

            // İlişkili etkinlikleri kontrol et
            var hasEvents = await _context.Etkinlikler.AnyAsync(e => e.KategoriId == id);
            if (hasEvents)
            {
                TempData["ErrorMessage"] = "Bu kategoriye ait etkinlikler olduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _context.Kategoriler.Remove(kategori);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }        private bool KategoriExists(int id)
        {
            return _context.Kategoriler.Any(e => e.KategoriId == id);
        }
    }
}
