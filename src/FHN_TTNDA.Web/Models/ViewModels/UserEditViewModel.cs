using System.ComponentModel.DataAnnotations;

namespace FHN_TTNDA.Web.Models.ViewModels;

public class UserEditViewModel
{
    public int? Id { get; set; }

    [Required, Display(Name = "İstifadəçi adı")]
    public string IstifadeciAdi { get; set; } = string.Empty;

    [Required, Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required, Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [Required, Display(Name = "FİN kod")]
    [RegularExpression(@"^[A-Za-z0-9]{7}$", ErrorMessage = "FİN kod tam 7 hərf/rəqəmdən ibarət olmalıdır.")]
    public string FinKod { get; set; } = string.Empty;

    [Display(Name = "Doğum tarixi")]
    [DataType(DataType.Date)]
    public DateOnly? DogumTarixi { get; set; }

    [Required(ErrorMessage = "Şöbə seçilməlidir"), Display(Name = "Şöbə")]
    public int? SobeId { get; set; }

    [Required(ErrorMessage = "Vəzifə seçilməlidir"), Display(Name = "Vəzifə")]
    public int? VezifeId { get; set; }

    [Display(Name = "Telefon")]
    [RegularExpression(@"^(\+994|0)(50|51|55|70|77|60|99|10|12)\d{7}$",
            ErrorMessage = "Telefon formatı düzgün deyil. Məsələn: +994501234567 və ya 0501234567")]
    public string? Telefon { get; set; }

    [Display(Name = "Daxili nömrə")]
    public string? DaxiliNomre { get; set; }

    [Required, EmailAddress, Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Şifrə")]
    [DataType(DataType.Password)]
    public string? Sifre { get; set; }

    public List<SobeVezifeSelectItem> Sobeler { get; set; } = new();
    public List<SobeVezifeSelectItem> Vezifeler { get; set; } = new();
}

public class SobeVezifeSelectItem
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
}
