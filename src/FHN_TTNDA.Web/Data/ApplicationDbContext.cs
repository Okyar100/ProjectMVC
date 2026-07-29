using FHN_TTNDA.Web.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FHN_TTNDA.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Sobe> Sobeler => Set<Sobe>();
    public DbSet<Vezife> Vezifeler => Set<Vezife>();
    public DbSet<Bolme> Bolmeler => Set<Bolme>();
    public DbSet<IstifadeciBolmeIcaze> IstifadeciBolmeIcazeleri => Set<IstifadeciBolmeIcaze>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("users");
            b.Property(u => u.Ad).HasColumnName("ad").HasMaxLength(100).IsRequired();
            b.Property(u => u.Soyad).HasColumnName("soyad").HasMaxLength(100).IsRequired();
            b.Property(u => u.FinKod).HasColumnName("fin_kod").HasMaxLength(20).IsRequired();
            b.Property(u => u.DogumTarixi).HasColumnName("dogum_tarixi");
            b.Property(u => u.SobeId).HasColumnName("sobe_id");
            b.Property(u => u.VezifeId).HasColumnName("vezife_id");
            b.Property(u => u.DaxiliNomre).HasColumnName("daxili_nomre").HasMaxLength(20);
            b.Property(u => u.YaradilmaTarixi).HasColumnName("yaradilma_tarixi");
            b.Property(u => u.Aktivdir).HasColumnName("aktivdir");

            b.HasIndex(u => u.FinKod).IsUnique();

            b.HasOne(u => u.Sobe)
                .WithMany(s => s.Istifadeciler)
                .HasForeignKey(u => u.SobeId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(u => u.Vezife)
                .WithMany(v => v.Istifadeciler)
                .HasForeignKey(u => u.VezifeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ApplicationRole>(b => b.ToTable("roles"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<int>>(b => b.ToTable("user_roles"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<int>>(b => b.ToTable("user_claims"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<int>>(b => b.ToTable("user_logins"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>>(b => b.ToTable("role_claims"));
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<int>>(b => b.ToTable("user_tokens"));


        builder.Entity<Sobe>(b =>
        {
            b.ToTable("sobeler");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Ad).HasColumnName("ad").HasMaxLength(200).IsRequired();
        });

        builder.Entity<Vezife>(b =>
        {
            b.ToTable("vezifeler");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Ad).HasColumnName("ad").HasMaxLength(200).IsRequired();
        });

        builder.Entity<Bolme>(b =>
        {
            b.ToTable("bolmeler");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Ad).HasColumnName("ad").HasMaxLength(100).IsRequired();
            b.Property(x => x.Kod).HasColumnName("kod").HasMaxLength(50).IsRequired();
            b.Property(x => x.Sira).HasColumnName("sira");
            b.HasIndex(x => x.Kod).IsUnique();
        });

        builder.Entity<IstifadeciBolmeIcaze>(b =>
        {
            b.ToTable("istifadeci_bolme_icazeleri");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.IstifadeciId).HasColumnName("istifadeci_id");
            b.Property(x => x.BolmeId).HasColumnName("bolme_id");
            b.Property(x => x.Baxis).HasColumnName("baxis");
            b.Property(x => x.Yeni).HasColumnName("yeni");
            b.Property(x => x.DuzelisEt).HasColumnName("duzelis_et");
            b.Property(x => x.AktivDeaktiv).HasColumnName("aktiv_deaktiv");
            b.Property(x => x.Icazeler).HasColumnName("icazeler");

            b.HasIndex(x => new { x.IstifadeciId, x.BolmeId }).IsUnique();

            b.HasOne(x => x.Istifadeci)
                .WithMany(u => u.Icazeler)
                .HasForeignKey(x => x.IstifadeciId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Bolme)
                .WithMany(bo => bo.Icazeler)
                .HasForeignKey(x => x.BolmeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
