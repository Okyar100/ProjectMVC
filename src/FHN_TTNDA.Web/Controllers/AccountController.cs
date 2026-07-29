using FHN_TTNDA.Web.Models.Entities;
using FHN_TTNDA.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FHN_TTNDA.Web.Controllers;

public class AccountController : Controller
{

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly FHN_TTNDA.Web.Data.ApplicationDbContext _db;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, FHN_TTNDA.Web.Data.ApplicationDbContext db)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _db = db;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Index", "Users");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
            return View(model);

        // Giris sadece Fin Kod ile heyata kecirilir.
        var girisDeyeri = model.UserNameOrEmail.Trim().ToUpperInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.FinKod == girisDeyeri);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Fin Kod və ya şifrə yanlışdır, ya da hesab deaktivdir.");
            return View(model);
        }

        if (!user.Aktivdir)
        {
            ModelState.AddModelError(string.Empty, "Hesabınız deaktiv edilib. Administratorla əlaqə saxlayın.");
            return View(model);
        }


        var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Users");

        if (result.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Hesab müvəqqəti bloklanıb. Bir az sonra yenidən cəhd edin.");
        else
            ModelState.AddModelError(string.Empty, "İstifadəçi adı/email və ya şifrə yanlışdır.");

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}
