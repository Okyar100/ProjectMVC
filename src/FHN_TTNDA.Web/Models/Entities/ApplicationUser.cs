using Microsoft.AspNetCore.Identity;

namespace FHN_TTNDA.Web.Models.Entities;


public class ApplicationUser : IdentityUser<int>
{
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;

    public string FinKod { get; set; } = string.Empty;

    public DateOnly? DogumTarixi { get; set; }

    public int? SobeId { get; set; }
    public Sobe? Sobe { get; set; }

    public int? VezifeId { get; set; }
    public Vezife? Vezife { get; set; }

    public string? DaxiliNomre { get; set; }

    public DateTime YaradilmaTarixi { get; set; } = DateTime.UtcNow;

    public bool Aktivdir { get; set; } = true;

    public ICollection<IstifadeciBolmeIcaze> Icazeler { get; set; } = new List<IstifadeciBolmeIcaze>();
}
