using FHN_TTNDA.Web.Data;
using FHN_TTNDA.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FHN_TTNDA.Web.ViewComponents;


public class SidebarViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public SidebarViewComponent(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var principal = HttpContext.User;
        if (principal?.Identity?.IsAuthenticated != true) return Content(string.Empty);

        var user = await _userManager.GetUserAsync(principal);
        if (user is null) return Content(string.Empty);

        var withRefs = await _db.Users
            .Include(u => u.Sobe)
            .Include(u => u.Vezife)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        return View(withRefs ?? user);
    }
}
