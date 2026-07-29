namespace FHN_TTNDA.Web.Models.ViewModels;


public class UserListItemViewModel
{
    public int Id { get; set; }
    public string IstifadeciAdi { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string FinKod { get; set; } = string.Empty;
    public DateOnly? DogumTarixi { get; set; }
    public string? Sobe { get; set; }
    public string? Vezife { get; set; }
    public string? Telefon { get; set; }
    public string? DaxiliNomre { get; set; }
    public string? Email { get; set; }
    public DateTime YaradilmaTarixi { get; set; }
    public bool Aktivdir { get; set; }
}
