using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace _221103018_OmerFarukBayraktutar.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IRezervasyonService _rezervasyonService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IEmailService emailService,
            IFileService fileService,
            IRezervasyonService rezervasyonService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _fileService = fileService;
            _rezervasyonService = rezervasyonService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = false,
                    KayitTarihi = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Kullanıcıya Customer rolü ata
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // E-posta doğrulama token oluştur
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id.ToString(), token = token },
                        protocol: HttpContext.Request.Scheme);

                    // E-posta gönder
                    await _emailService.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    TempData["SuccessMessage"] = "Kayıt başarılı! Lütfen e-posta adresinizi onaylayın.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"ID'si {userId} olan kullanıcı bulunamadı.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "E-posta adresiniz onaylandı. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "E-posta onaylanırken bir hata oluştu.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "E-posta adresi veya şifre hatalı.");
                    return View(model);
                }
                
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "E-posta adresi onaylanmamış. Lütfen e-postanızı kontrol edin.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    // Kullanıcının rolünü kontrol et ve uygun alana yönlendir
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Organizer"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Organizator" });
                    }
                    
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "E-posta adresi veya şifre hatalı.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Kullanıcı bulunamadığında veya e-posta doğrulanmadığında bile güvenlik için aynı mesajı göster
                    TempData["InfoMessage"] = "Sistemde kayıtlı e-posta adresi bulunamadı veya doğrulanmamış.";
                    return RedirectToAction("Login");
                }

                // Geliştirme ortamı için, doğrudan şifre sıfırlama - GERÇEKCİ BİR ORTAMDA KULLANMAYIN!
                string newPassword = GenerateRandomPassword();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    // Şifreyi doğrudan göster (sadece geliştirme ortamı için!)
                    TempData["SuccessMessage"] = $"Yeni şifreniz: {newPassword} <br><strong>NOT: Bu özellik sadece geliştirme ortamında kullanılmaktadır!</strong>";
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            return View(model);
        }

        // Rastgele şifre oluşturan yardımcı metot
        private string GenerateRandomPassword()
        {
            // Rastgele şifre oluştur (8 karakter uzunluğunda, büyük-küçük harf, rakam ve özel karakter içeren)
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";
            var random = new Random();
            var chars = new char[8];
            
            // En az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermesini sağla
            chars[0] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(26)];
            chars[1] = "abcdefghijklmnopqrstuvwxyz"[random.Next(26)];
            chars[2] = "0123456789"[random.Next(10)];
            chars[3] = "!@#$%^&*()-_=+"[random.Next(13)];
            
            // Geri kalan karakterleri rastgele doldur
            for (int i = 4; i < 8; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }
            
            // Karakterleri karıştır
            for (int i = 0; i < 8; i++)
            {
                int j = random.Next(8);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
            
            return new string(chars);
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordViewModel
            {
                Code = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile(string tab = "profile-info")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcının rezervasyonlarını getir
            var userRezervasyonlar = await _rezervasyonService.GetKullaniciRezervasyonlarAsync(user.Id);

            var model = new ProfileViewModel
            {
                Id = user.Id.ToString(),
                Ad = user.Ad,
                Soyad = user.Soyad,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilResimYolu = user.ProfilResimYolu,
                Rezervasyonlar = userRezervasyonlar
            };

            // Tab değerini ViewBag ile geçelim
            ViewBag.ActiveTab = tab;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.Ad = model.Ad;
                user.Soyad = model.Soyad;
                user.PhoneNumber = model.PhoneNumber;                // Profil resmi yükleme
                if (model.NewProfileImage != null)
                {
                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(model.ProfilResimYolu))
                    {
                        await _fileService.DeleteFileAsync(model.ProfilResimYolu);
                    }

                    // Yeni resmi kaydet
                    user.ProfilResimYolu = await _fileService.SaveUserImageAsync(model.NewProfileImage);
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profil bilgileriniz güncellendi.";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Tüm alanları doldurunuz.";
                return RedirectToAction("Profile", new { tab = "password" });
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Yeni şifreler eşleşmiyor.";
                return RedirectToAction("Profile", new { tab = "password" });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    TempData["ErrorMessage"] = error.Description;
                }
                return RedirectToAction("Profile", new { tab = "password" });
            }

            // Şifre değiştiğinde oturumu yenile
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Profile", new { tab = "password" });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Biletlerim()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // RezervasyonService'e erişimimiz olmadığı için burada bir IRezervasyonService arayüzünü 
            // constructor'da enjekte etmemiz gerekir, ancak şimdilik Profile sayfasına yönlendirelim
            // ve oradaki Biletlerim tab'ını açalım
            return RedirectToAction("Profile", new { tab = "reservations" });
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
