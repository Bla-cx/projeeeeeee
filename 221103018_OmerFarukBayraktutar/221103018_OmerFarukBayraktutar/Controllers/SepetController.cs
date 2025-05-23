using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace _221103018_OmerFarukBayraktutar.Controllers
{
    [Authorize]
    public class SepetController : Controller
    {
        private readonly ISepetService _sepetService;
        private readonly IRezervasyonService _rezervasyonService;
        private readonly IPaymentService _paymentService;

        public SepetController(
            ISepetService sepetService, 
            IRezervasyonService rezervasyonService,
            IPaymentService paymentService)
        {
            _sepetService = sepetService;
            _rezervasyonService = rezervasyonService;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            var sepetItems = await _sepetService.GetKullaniciSepetAsync(userIdInt);
            var toplamTutar = await _sepetService.GetSepetToplamAsync(userIdInt);

            var viewModel = new SepetViewModel
            {
                SepetOgeleri = sepetItems,
                ToplamTutar = toplamTutar
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int etkinlikId, int biletSayisi = 1)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userIdInt = int.Parse(userId);
                await _sepetService.AddToCartAsync(userIdInt, etkinlikId, biletSayisi);
                TempData["SuccessMessage"] = "Etkinlik sepete eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detay", "Etkinlik", new { id = etkinlikId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int sepetId)
        {
            await _sepetService.RemoveFromCartAsync(sepetId);
            TempData["SuccessMessage"] = "Ürün sepetten kaldırıldı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int sepetId, int biletSayisi)
        {
            try
            {
                await _sepetService.UpdateBiletSayisiAsync(sepetId, biletSayisi);
                TempData["SuccessMessage"] = "Sepet güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdInt = int.Parse(userId);
            await _sepetService.ClearCartAsync(userIdInt);
            TempData["SuccessMessage"] = "Sepet temizlendi.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var userIdInt = int.Parse(userId);
            var sepetItems = await _sepetService.GetKullaniciSepetAsync(userIdInt);
            var toplamTutar = await _sepetService.GetSepetToplamAsync(userIdInt);

            var viewModel = new CheckoutViewModel
            {
                SepetOgeleri = sepetItems,
                ToplamTutar = toplamTutar
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteCheckout(CheckoutViewModel model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userIdInt = int.Parse(userId);
                var sepetItems = await _sepetService.GetKullaniciSepetAsync(userIdInt);
                
                if (!sepetItems.Any())
                {
                    ModelState.AddModelError("", "Sepetiniz boş.");
                    return View("Checkout", model);
                }
                
                var toplamTutar = await _sepetService.GetSepetToplamAsync(userIdInt);

                // Ödeme işlemi
                var paymentModel = new PaymentViewModel
                {
                    CardNumber = model.CardNumber,
                    CardHolder = model.CardHolder,
                    ExpiryMonth = model.ExpiryMonth,
                    ExpiryYear = model.ExpiryYear,
                    CVV = model.CVV,
                    Amount = toplamTutar
                };

                var paymentResult = await _paymentService.ProcessPaymentAsync(paymentModel);

                if (!paymentResult.IsSuccessful)
                {
                    ModelState.AddModelError("", "Ödeme işlemi başarısız: " + paymentResult.ErrorMessage);
                    model.SepetOgeleri = sepetItems;
                    model.ToplamTutar = toplamTutar;
                    return View("Checkout", model);
                }
                
                // Rezervasyonları oluştur
                try
                {
                    foreach (var item in sepetItems)
                    {
                        await _rezervasyonService.CreateRezervasyonAsync(
                            item.EtkinlikId, 
                            userIdInt, 
                            item.BiletSayisi);
                    }

                    // Sepeti temizle
                    await _sepetService.ClearCartAsync(userIdInt);

                    // Ödeme sonuç ekranına yönlendir
                    TempData["SuccessMessage"] = "Ödeme başarılı! Rezervasyonunuz oluşturuldu.";
                    TempData["PaymentId"] = paymentResult.TransactionId;
                    TempData["PaymentAmount"] = paymentResult.PaymentAmount.ToString();
                    TempData["PaymentDate"] = paymentResult.PaymentDate;
                    TempData["CardLast4"] = paymentResult.CardLast4Digits;
                    
                    return RedirectToAction("OdemeBasarili");
                }
                catch (Exception ex)
                {
                    // Rezervasyon oluşturma hatası
                    ModelState.AddModelError("", "Rezervasyon oluşturulurken bir hata oluştu: " + ex.Message);
                    model.SepetOgeleri = sepetItems;
                    model.ToplamTutar = toplamTutar;
                    return View("Checkout", model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ödeme işlemi sırasında bir hata oluştu: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }

        public IActionResult OdemeBasarili()
        {
            if (TempData["PaymentId"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new PaymentSuccessViewModel
            {
                TransactionId = TempData["PaymentId"].ToString(),
                Amount = Convert.ToDecimal(TempData["PaymentAmount"]),
                PaymentDate = Convert.ToDateTime(TempData["PaymentDate"]),
                CardLast4Digits = TempData["CardLast4"].ToString()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSepetCount()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(0);
            }

            var userIdInt = int.Parse(userId);
            var count = await _sepetService.GetSepetCountAsync(userIdInt);
            return Json(count);
        }
    }
}
