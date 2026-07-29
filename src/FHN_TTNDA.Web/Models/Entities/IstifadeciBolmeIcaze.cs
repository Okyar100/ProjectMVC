namespace FHN_TTNDA.Web.Models.Entities;

public class IstifadeciBolmeIcaze
{
    public int Id { get; set; }

    public int IstifadeciId { get; set; }
    public ApplicationUser Istifadeci { get; set; } = null!;

    public int BolmeId { get; set; }
    public Bolme Bolme { get; set; } = null!;

    public bool Baxis { get; set; }
    public bool Yeni { get; set; }
    public bool DuzelisEt { get; set; }
    public bool AktivDeaktiv { get; set; }
    public bool Icazeler { get; set; }
}
