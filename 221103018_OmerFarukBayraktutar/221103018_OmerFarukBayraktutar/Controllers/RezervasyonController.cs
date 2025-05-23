using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Controllers
{
    [Authorize]
    public class RezervasyonController : Controller
    {
        private readonly IRezervasyonService _rezervasyonService;

        public RezervasyonController(IRezervasyonService rezervasyonService)
        {
            _rezervasyonService = rezervasyonService;
        }

        public async Task<IActionResult> Rezervasyonlarim()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            var rezervasyonlar = await _rezervasyonService.GetKullaniciRezervasyonlarAsync(userIdInt);
            return View(rezervasyonlar);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var rezervasyon = await _rezervasyonService.GetRezervasyonDetayAsync(id);
            
            if (rezervasyon == null)
            {
                return NotFound();
            }

            // Kullanıcı kendi rezervasyonunu görebilir
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userIdInt = int.Parse(userId);
            if (rezervasyon.KullaniciId != userIdInt && !User.IsInRole("Admin") && !User.IsInRole("Organizer"))
            {
                return Forbid();
            }

            return View(rezervasyon);
        }

        [HttpPost]
        public async Task<IActionResult> Iptal(int id)
        {
            try
            {
                await _rezervasyonService.IptalEtAsync(id);
                TempData["SuccessMessage"] = "Rezervasyon başarıyla iptal edildi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            
            return RedirectToAction("Rezervasyonlarim");
        }

        [HttpGet]
        public IActionResult BiletKontrol()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BiletKontrol(string barkodNo)
        {
            if (string.IsNullOrEmpty(barkodNo))
            {
                TempData["ErrorMessage"] = "Barkod numarası boş olamaz.";
                return View();
            }

            var gecerli = await _rezervasyonService.BiletKontrolAsync(barkodNo);
            
            if (gecerli)
            {
                TempData["SuccessMessage"] = "Bilet geçerlidir.";
            }
            else
            {
                TempData["ErrorMessage"] = "Bilet geçersiz veya kullanılmış.";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IptalEt(int rezervasyonId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Debug için rezervasyon ID'sini loglama
                System.Diagnostics.Debug.WriteLine($"IptalEt called with rezervasyonId: {rezervasyonId}");
                
                // Rezervasyon var mı ve bu kullanıcıya mı ait kontrol et
                var rezervasyon = await _rezervasyonService.GetRezervasyonDetayAsync(rezervasyonId);
                
                if (rezervasyon == null)
                {
                    TempData["ErrorMessage"] = "Rezervasyon bulunamadı.";
                    return RedirectToAction("Biletlerim", "Account");
                }
                
                var userIdInt = int.Parse(userId);
                if (rezervasyon.KullaniciId != userIdInt && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "Bu işlem için yetkiniz bulunmamaktadır.";
                    return RedirectToAction("Biletlerim", "Account");
                }
                
                // Etkinliğe 24 saatten az kaldıysa iptal edemez
                if (rezervasyon.Etkinlik.Tarih <= DateTime.Now.AddDays(1))
                {
                    TempData["ErrorMessage"] = "Etkinliğe 24 saatten az kaldığı için iptal işlemi yapılamaz.";
                    return RedirectToAction("Biletlerim", "Account", new { tab = "reservations" });
                }
                
                var result = await _rezervasyonService.IptalEtAsync(rezervasyonId);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Rezervasyonunuz başarıyla iptal edildi. Ödeme iadesi 7 iş günü içinde yapılacaktır.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Rezervasyon iptal edilirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }
            
            return RedirectToAction("Biletlerim", "Account", new { tab = "reservations" });
        }
    }
}
