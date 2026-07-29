using FHN_TTNDA.Web.Models;
using FHN_TTNDA.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FHN_TTNDA.Web.Data;


public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        // 1) Bölmələr
        var bolmeler = new (string Kod, string Ad, int Sira)[]
        {
            (BolmeKodlari.Istifadeciler, "İstifadəçilər", 1),
        };

        foreach (var b in bolmeler)
        {
            if (!await context.Bolmeler.AnyAsync(x => x.Kod == b.Kod))
            {
                context.Bolmeler.Add(new Bolme { Kod = b.Kod, Ad = b.Ad, Sira = b.Sira });
            }
        }
        await context.SaveChangesAsync();

        // 2) Rollar
        foreach (var rol in new[] { "Admin", "Istifadeci" })
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new ApplicationRole(rol));
        }

        // 3) Şöbə / Vəzifə məlumatları
        if (!await context.Sobeler.AnyAsync())
        {
            context.Sobeler.Add(new Sobe { Ad = "FHN TTNDA" });
            await context.SaveChangesAsync();
        }
        if (!await context.Vezifeler.AnyAsync())
        {
            context.Vezifeler.Add(new Vezife { Ad = "Mühəndis-proqramçı" });
            await context.SaveChangesAsync();
        }

        // 4) İlk admin istifadəçi (screenshot-dakı "İbrahim İbrahimli")
        const string adminEmail = "ibrahimli@fhnttnda.az";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var sobe = await context.Sobeler.FirstAsync();
            var vezife = await context.Vezifeler.FirstAsync();

            var admin = new ApplicationUser
            {
                UserName = "SHUAK8Z",
                Email = adminEmail,
                Ad = "İbrahim",
                Soyad = "İbrahimli",
                FinKod = "SHUAK8Z",
                SobeId = sobe.Id,
                VezifeId = vezife.Id,
                Aktivdir = true,
                EmailConfirmed = true,
                YaradilmaTarixi = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");

                // Full accessed
                var hamiBolmeler = await context.Bolmeler.ToListAsync();
                foreach (var bolme in hamiBolmeler)
                {
                    context.IstifadeciBolmeIcazeleri.Add(new IstifadeciBolmeIcaze
                    {
                        IstifadeciId = admin.Id,
                        BolmeId = bolme.Id,
                        Baxis = true,
                        Yeni = true,
                        DuzelisEt = true,
                        AktivDeaktiv = true,
                        Icazeler = true,
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
