using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Ad = user.Ad,
                    Soyad = user.Soyad,
                    Telefon = user.Telefon,
                    KayitTarihi = user.KayitTarihi,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Telefon = user.Telefon,
                KayitTarihi = user.KayitTarihi,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };

            return View(userViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Telefon = user.Telefon,
                EmailConfirmed = user.EmailConfirmed,
                UserRoles = roles.ToList(),
                AllRoles = allRoles.Select(r => r.Name).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Ad = model.Ad;
                user.Soyad = model.Soyad;
                user.Telefon = model.Telefon;
                user.EmailConfirmed = model.EmailConfirmed;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Rolleri güncelle
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    
                    // Kaldırılacak roller
                    var rolesToRemove = currentRoles.Where(r => !model.SelectedRoles.Contains(r)).ToList();
                    if (rolesToRemove.Any())
                    {
                        result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", "Roller kaldırılırken bir hata oluştu.");
                            return View(model);
                        }
                    }

                    // Eklenecek roller
                    var rolesToAdd = model.SelectedRoles.Where(r => !currentRoles.Contains(r)).ToList();
                    if (rolesToAdd.Any())
                    {
                        result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", "Roller eklenirken bir hata oluştu.");
                            return View(model);
                        }
                    }

                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Rolleri tekrar yükle
            var allRoles = await _roleManager.Roles.ToListAsync();
            model.AllRoles = allRoles.Select(r => r.Name).ToList();

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Ad = user.Ad,
                Soyad = user.Soyad,
                Telefon = user.Telefon,
                KayitTarihi = user.KayitTarihi,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };

            return View(userViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Admin kullanıcısını silmeyi engelle
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin && (await _userManager.GetUsersInRoleAsync("Admin")).Count <= 1)
            {
                TempData["ErrorMessage"] = "Son admin kullanıcısı silinemez!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Delete", new UserViewModel { Id = user.Id });
        }

        public async Task<IActionResult> ConfirmEmail(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "E-posta adresi onaylandı.";
            }
            else
            {
                TempData["ErrorMessage"] = "E-posta onaylanırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
