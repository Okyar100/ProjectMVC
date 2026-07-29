namespace FHN_TTNDA.Web.Models.Entities;

public class Vezife
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public ICollection<ApplicationUser> Istifadeciler { get; set; } = new List<ApplicationUser>();
}
