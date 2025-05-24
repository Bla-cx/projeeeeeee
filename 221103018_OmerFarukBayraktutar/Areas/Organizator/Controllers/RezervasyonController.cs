using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Organizator.Controllers
{    [Area("Organizator")]
    [Authorize(Roles = "Organizator")]
    public class RezervasyonController : Controller
    {
        private readonly EtkinliklerDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RezervasyonController(EtkinliklerDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Tüm rezervasyonları listeler
        public async Task<IActionResult> Index()
        {
            // Kullanıcının organizatör olduğu etkinliklerin rezervasyonlarını getir
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            // String userId'yi int'e dönüştür
            int organizatorId = int.Parse(userId);

            var rezervasyonlar = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .Include(r => r.Kullanici)
                .Where(r => r.Etkinlik.OrganizatorId == organizatorId)
                .OrderByDescending(r => r.RezervasyonTarihi)
                .ToListAsync();

            return View(rezervasyonlar);
        }

        // Rezervasyon detaylarını gösterir
        public async Task<IActionResult> Detay(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            // String userId'yi int'e dönüştür
            int organizatorId = int.Parse(userId);

            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .ThenInclude(e => e.Kategori)
                .Include(r => r.Etkinlik.Sehir)
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.RezervasyonId == id && r.Etkinlik.OrganizatorId == organizatorId);

            if (rezervasyon == null)
            {
                return NotFound();
            }

            return View(rezervasyon);
        }

        // Bilet kontrol sayfasını gösterir
        [HttpGet]
        public IActionResult BiletKontrol()
        {
            return View();
        }

        // Bilet kontrolü için POST işlemi
        [HttpPost]
        public async Task<IActionResult> BiletKontrol(string barkodNo)
        {
            if (string.IsNullOrEmpty(barkodNo))
            {
                TempData["ErrorMessage"] = "Bilet kodu boş olamaz.";
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            // String userId'yi int'e dönüştür
            int organizatorId = int.Parse(userId);

            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .ThenInclude(e => e.Kategori)
                .Include(r => r.Etkinlik.Sehir)
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.BarkodNo == barkodNo && r.Etkinlik.OrganizatorId == organizatorId);

            if (rezervasyon == null)
            {
                TempData["ErrorMessage"] = "Bilet kodu geçersiz veya size ait değil.";
                return View();
            }

            // Etkinlik tarihi geçmiş mi kontrol et
            if (rezervasyon.Etkinlik.Tarih.Date < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Bu etkinlik tarihinde geçmiş, bilet geçersiz.";
                return View();
            }
            
            // Etkinlik bugün ise ve başlangıç saati geçmiş mi kontrol et
            if (rezervasyon.Etkinlik.Tarih.Date == DateTime.Now.Date && 
                rezervasyon.Etkinlik.BaslangicSaati < TimeSpan.FromHours(DateTime.Now.Hour) + TimeSpan.FromMinutes(DateTime.Now.Minute))
            {
                TempData["ErrorMessage"] = "Bu etkinliğin başlangıç saati geçmiş, bilet geçersiz.";
                return View();
            }

            return View("BiletKontrolSonuc", rezervasyon);
        }

        // Bileti kullanıldı olarak işaretler
        [HttpPost]
        public async Task<IActionResult> Kullanildi(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            // String userId'yi int'e dönüştür
            int organizatorId = int.Parse(userId);

            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .FirstOrDefaultAsync(r => r.RezervasyonId == id && r.Etkinlik.OrganizatorId == organizatorId);

            if (rezervasyon == null)
            {
                TempData["ErrorMessage"] = "Rezervasyon bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (rezervasyon.Etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bilgileri bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (rezervasyon.Iptal)
            {
                TempData["ErrorMessage"] = "Bu bilet iptal edilmiş durumda, kullanılamaz.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            if (rezervasyon.Kullanildi)
            {
                TempData["ErrorMessage"] = "Bu bilet zaten kullanılmış durumda.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            try
            {
                // Bileti kullanıldı olarak işaretle
                rezervasyon.Kullanildi = true;

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Bilet başarıyla kullanıldı olarak işaretlendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İşlem sırasında bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Detay), new { id });
        }

        // Bileti kullanıldı olarak işaretler (QR kodu tarama sonrası)
        [HttpPost]
        public async Task<IActionResult> BiletKullan(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            // String userId'yi int'e dönüştür
            int organizatorId = int.Parse(userId);

            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .FirstOrDefaultAsync(r => r.RezervasyonId == id && r.Etkinlik.OrganizatorId == organizatorId);

            if (rezervasyon == null)
            {
                TempData["ErrorMessage"] = "Rezervasyon bulunamadı.";
                return RedirectToAction(nameof(BiletKontrol));
            }

            if (rezervasyon.Etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bilgileri bulunamadı.";
                return RedirectToAction(nameof(BiletKontrol));
            }

            if (rezervasyon.Iptal)
            {
                TempData["ErrorMessage"] = "Bu bilet iptal edilmiş durumda, kullanılamaz.";
                return RedirectToAction(nameof(BiletKontrol));
            }

            if (rezervasyon.Kullanildi)
            {
                TempData["ErrorMessage"] = "Bu bilet zaten kullanılmış durumda.";
                return RedirectToAction(nameof(BiletKontrol));
            }

            try
            {
                // Bileti kullanıldı olarak işaretle
                rezervasyon.Kullanildi = true;

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Bilet başarıyla kullanıldı olarak işaretlendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "İşlem sırasında bir hata oluştu: " + ex.Message;
            }
            
            return RedirectToAction(nameof(BiletKontrol));
        }

        // Bileti iptal eder
        [HttpPost]
        public async Task<IActionResult> Iptal(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();

            // String userId'yi int'e dönüştür
            int organizatorId = int.Parse(userId);

            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .FirstOrDefaultAsync(r => r.RezervasyonId == id && r.Etkinlik.OrganizatorId == organizatorId);

            if (rezervasyon == null)
            {
                TempData["ErrorMessage"] = "Rezervasyon bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (rezervasyon.Etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bilgileri bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (rezervasyon.Iptal)
            {
                TempData["ErrorMessage"] = "Bu bilet zaten iptal edilmiş durumda.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            if (rezervasyon.Kullanildi)
            {
                TempData["ErrorMessage"] = "Kullanılmış bir bilet iptal edilemez.";
                return RedirectToAction(nameof(Detay), new { id });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Bileti iptal et
                rezervasyon.Iptal = true;

                // Etkinlik kontenjanını güncelle
                rezervasyon.Etkinlik.KalanKapasite += rezervasyon.BiletSayisi;

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Bilet başarıyla iptal edildi.";
                return RedirectToAction(nameof(Detay), new { id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Bilet iptal edilirken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Detay), new { id });
            }
        }
    }
} 