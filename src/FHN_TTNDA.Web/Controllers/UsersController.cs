using FHN_TTNDA.Web.Data;
using FHN_TTNDA.Web.Models;
using FHN_TTNDA.Web.Models.Entities;
using FHN_TTNDA.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FHN_TTNDA.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private async Task<bool> HasPermissionAsync(string permission)
    {
        if (User.IsInRole("Admin")) return true;

        var userIdStr = _userManager.GetUserId(User);
        if (userIdStr is null || !int.TryParse(userIdStr, out var userId)) return false;

        var icaze = await _db.IstifadeciBolmeIcazeleri
            .Include(x => x.Bolme)
            .FirstOrDefaultAsync(x => x.IstifadeciId == userId && x.Bolme.Kod == BolmeKodlari.Istifadeciler);

        if (icaze is null) return false;

        return permission switch
        {
            "Baxis" => icaze.Baxis,
            "Yeni" => icaze.Yeni,
            "DuzelisEt" => icaze.DuzelisEt,
            "AktivDeaktiv" => icaze.AktivDeaktiv,
            "Icazeler" => icaze.Icazeler,
            _ => false
        };
    }

    public async Task<IActionResult> Index()
    {
        var canBaxis = await HasPermissionAsync("Baxis");
        ViewBag.CanBaxis = canBaxis;

        if (!canBaxis)
        {
            return View(new List<UserListItemViewModel>());
        }

        var list = await _db.Users
            .Include(u => u.Sobe)
            .Include(u => u.Vezife)
            .OrderBy(u => u.UserName)
            .Select(u => new UserListItemViewModel
            {
                Id = u.Id,
                IstifadeciAdi = u.UserName ?? string.Empty,
                Ad = u.Ad,
                Soyad = u.Soyad,
                FinKod = u.FinKod,
                DogumTarixi = u.DogumTarixi,
                Sobe = u.Sobe != null ? u.Sobe.Ad : null,
                Vezife = u.Vezife != null ? u.Vezife.Ad : null,
                Telefon = u.PhoneNumber,
                DaxiliNomre = u.DaxiliNomre,
                Email = u.Email,
                YaradilmaTarixi = u.YaradilmaTarixi,
                Aktivdir = u.Aktivdir
            })
            .ToListAsync();

        ViewBag.CanYeni = await HasPermissionAsync("Yeni");
        ViewBag.CanDuzelis = await HasPermissionAsync("DuzelisEt");
        ViewBag.CanIcazeler = await HasPermissionAsync("Icazeler");
        ViewBag.CanAktivDeaktiv = await HasPermissionAsync("AktivDeaktiv");

        return View(list);
    }


    public async Task<IActionResult> Create()
    {
        if (!await HasPermissionAsync("Yeni")) return RedirectToAction("AccessDenied", "Account");

        var vm = new UserEditViewModel
        {
            Sobeler = await _db.Sobeler.Select(s => new SobeVezifeSelectItem { Id = s.Id, Ad = s.Ad }).ToListAsync(),
            Vezifeler = await _db.Vezifeler.Select(v => new SobeVezifeSelectItem { Id = v.Id, Ad = v.Ad }).ToListAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserEditViewModel model)
    {
        model.FinKod = model.FinKod?.Trim().ToUpperInvariant() ?? string.Empty;

        if (!await HasPermissionAsync("Yeni")) return RedirectToAction("AccessDenied", "Account");

        if (await _db.Users.AnyAsync(u => u.UserName == model.IstifadeciAdi))
            ModelState.AddModelError(nameof(model.IstifadeciAdi), "Bu istifadəçi adı artıq mövcuddur.");

        if (await _db.Users.AnyAsync(u => u.FinKod == model.FinKod))
            ModelState.AddModelError(nameof(model.FinKod), "Bu FİN kod artıq başqa istifadəçiyə aiddir.");

        if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "Bu email artıq istifadə olunub.");

        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.IstifadeciAdi,
            Email = model.Email,
            Ad = model.Ad,
            Soyad = model.Soyad,
            FinKod = model.FinKod,
            DogumTarixi = model.DogumTarixi,
            SobeId = model.SobeId,
            VezifeId = model.VezifeId,
            PhoneNumber = model.Telefon,
            DaxiliNomre = model.DaxiliNomre,
            Aktivdir = true,
            EmailConfirmed = true,
            YaradilmaTarixi = DateTime.UtcNow
        };

        var sifre = string.IsNullOrWhiteSpace(model.Sifre) ? "1234" : model.Sifre;
        var result = await _userManager.CreateAsync(user, sifre);

        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, "Istifadeci");
        TempData["Success"] = "İstifadəçi uğurla yaradıldı.";
        return RedirectToAction(nameof(Index));
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.IsInRole("Admin")) return RedirectToAction("AccessDenied", "Account");

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var cariIstifadeciId = _userManager.GetUserId(User);
        if (cariIstifadeciId != null && int.Parse(cariIstifadeciId) == id)
        {
            TempData["Error"] = "Öz hesabınızı silə bilməzsiniz.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.DeleteAsync(user);
        TempData["Success"] = "İstifadəçi silindi.";
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Edit(int id)
    {
        if (!await HasPermissionAsync("DuzelisEt")) return RedirectToAction("AccessDenied", "Account");

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            IstifadeciAdi = user.UserName ?? string.Empty,
            Ad = user.Ad,
            Soyad = user.Soyad,
            FinKod = user.FinKod,
            DogumTarixi = user.DogumTarixi,
            SobeId = user.SobeId,
            VezifeId = user.VezifeId,
            Telefon = user.PhoneNumber,
            DaxiliNomre = user.DaxiliNomre,
            Email = user.Email ?? string.Empty
        };
        await PopulateSelectListsAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserEditViewModel model)
    {
        if (!await HasPermissionAsync("DuzelisEt")) return RedirectToAction("AccessDenied", "Account");

        if (id != model.Id) return BadRequest();

        model.FinKod = model.FinKod?.Trim().ToUpperInvariant() ?? string.Empty;

        if (await _db.Users.AnyAsync(u => u.UserName == model.IstifadeciAdi && u.Id != id))
            ModelState.AddModelError(nameof(model.IstifadeciAdi), "Bu istifadəçi adı artıq mövcuddur.");

        if (await _db.Users.AnyAsync(u => u.FinKod == model.FinKod && u.Id != id))
            ModelState.AddModelError(nameof(model.FinKod), "Bu FİN kod artıq başqa istifadəçiyə aiddir.");

        if (await _db.Users.AnyAsync(u => u.Email == model.Email && u.Id != id))
            ModelState.AddModelError(nameof(model.Email), "Bu email artıq istifadə olunub.");

        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.UserName = model.IstifadeciAdi;
        user.NormalizedUserName = model.IstifadeciAdi.ToUpperInvariant();
        user.Email = model.Email;
        user.NormalizedEmail = model.Email.ToUpperInvariant();
        user.Ad = model.Ad;
        user.Soyad = model.Soyad;
        user.FinKod = model.FinKod;
        user.DogumTarixi = model.DogumTarixi;
        user.SobeId = model.SobeId;
        user.VezifeId = model.VezifeId;
        user.PhoneNumber = model.Telefon;
        user.DaxiliNomre = model.DaxiliNomre;

        if (!string.IsNullOrWhiteSpace(model.Sifre))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, model.Sifre);
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "İstifadəçi məlumatları yeniləndi.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        if (!await HasPermissionAsync("AktivDeaktiv")) return RedirectToAction("AccessDenied", "Account");

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.Aktivdir = !user.Aktivdir;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }



    public async Task<IActionResult> Permissions(int id, string? bolme = null)
    {
        if (!await HasPermissionAsync("Icazeler")) return RedirectToAction("AccessDenied", "Account");

        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        var bolmeler = await _db.Bolmeler.OrderBy(b => b.Sira).ToListAsync();
        var secilmisKod = bolme ?? bolmeler.FirstOrDefault(b => b.Kod == BolmeKodlari.Istifadeciler)?.Kod ?? bolmeler.First().Kod;
        var secilmisBolme = bolmeler.First(b => b.Kod == secilmisKod);

        var icaze = await _db.IstifadeciBolmeIcazeleri
            .FirstOrDefaultAsync(x => x.IstifadeciId == id && x.BolmeId == secilmisBolme.Id);

        var vm = new UserPermissionsViewModel
        {
            IstifadeciId = user.Id,
            IstifadeciAdi = $"{user.Ad} {user.Soyad}",
            FinKod = user.FinKod,
            Bolmeler = bolmeler.Select(b => new BolmeItemViewModel { Id = b.Id, Kod = b.Kod, Ad = b.Ad }).ToList(),
            SecilmisBolmeKodu = secilmisKod,
            Icaze = new BolmeIcazeItemViewModel
            {
                Baxis = icaze?.Baxis ?? false,
                Yeni = icaze?.Yeni ?? false,
                DuzelisEt = icaze?.DuzelisEt ?? false,
                AktivDeaktiv = icaze?.AktivDeaktiv ?? false,
                Icazeler = icaze?.Icazeler ?? false
            }
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissions(int id, string bolmeKodu, BolmeIcazeItemViewModel icaze)
    {
        if (!await HasPermissionAsync("Icazeler")) return RedirectToAction("AccessDenied", "Account");

        // "Baxış" olmadan başqa heç bir icazə verilə bilməz
        if ((icaze.Yeni || icaze.DuzelisEt || icaze.AktivDeaktiv || icaze.Icazeler) && !icaze.Baxis)
        {
            TempData["Error"] = "Digər icazələri vermək üçün əvvəlcə \"Baxış\" icazəsi seçilməlidir.";
            return RedirectToAction(nameof(Permissions), new { id, bolme = bolmeKodu });
        }

        var bolme = await _db.Bolmeler.FirstOrDefaultAsync(b => b.Kod == bolmeKodu);
        if (bolme is null) return NotFound();

        var movcud = await _db.IstifadeciBolmeIcazeleri
            .FirstOrDefaultAsync(x => x.IstifadeciId == id && x.BolmeId == bolme.Id);

        if (movcud is null)
        {
            movcud = new IstifadeciBolmeIcaze { IstifadeciId = id, BolmeId = bolme.Id };
            _db.IstifadeciBolmeIcazeleri.Add(movcud);
        }

        movcud.Baxis = icaze.Baxis;
        movcud.Yeni = icaze.Yeni;
        movcud.DuzelisEt = icaze.DuzelisEt;
        movcud.AktivDeaktiv = icaze.AktivDeaktiv;
        movcud.Icazeler = icaze.Icazeler;

        await _db.SaveChangesAsync();
        TempData["Success"] = "İcazələr yadda saxlanıldı.";
        return RedirectToAction(nameof(Permissions), new { id, bolme = bolmeKodu });
    }

    private async Task PopulateSelectListsAsync(UserEditViewModel model)
    {
        model.Sobeler = await _db.Sobeler.Select(s => new SobeVezifeSelectItem { Id = s.Id, Ad = s.Ad }).ToListAsync();
        model.Vezifeler = await _db.Vezifeler.Select(v => new SobeVezifeSelectItem { Id = v.Id, Ad = v.Ad }).ToListAsync();
    }

}
