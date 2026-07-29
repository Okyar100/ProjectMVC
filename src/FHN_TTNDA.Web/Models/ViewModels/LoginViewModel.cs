using System.ComponentModel.DataAnnotations;

namespace FHN_TTNDA.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "FİN kod daxil edin")]
    [Display(Name = "FİN kod")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifrə daxil edin")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifrə")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Məni xatırla")]
    public bool RememberMe { get; set; }
}
