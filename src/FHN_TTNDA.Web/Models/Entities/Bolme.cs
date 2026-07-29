namespace FHN_TTNDA.Web.Models.Entities;

public class Bolme
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;

    public string Kod { get; set; } = string.Empty;

    public int Sira { get; set; }

    public ICollection<IstifadeciBolmeIcaze> Icazeler { get; set; } = new List<IstifadeciBolmeIcaze>();
}
