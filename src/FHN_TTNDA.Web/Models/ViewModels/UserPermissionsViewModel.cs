namespace FHN_TTNDA.Web.Models.ViewModels;

public class UserPermissionsViewModel
{
    public int IstifadeciId { get; set; }
    public string IstifadeciAdi { get; set; } = string.Empty;
    public string FinKod { get; set; } = string.Empty;

    public List<BolmeItemViewModel> Bolmeler { get; set; } = new();


    public string SecilmisBolmeKodu { get; set; } = string.Empty;

    public BolmeIcazeItemViewModel Icaze { get; set; } = new();
}

public class BolmeItemViewModel
{
    public int Id { get; set; }
    public string Kod { get; set; } = string.Empty;
    public string Ad { get; set; } = string.Empty;
}

public class BolmeIcazeItemViewModel
{
    public bool Baxis { get; set; }
    public bool Yeni { get; set; }
    public bool DuzelisEt { get; set; }
    public bool AktivDeaktiv { get; set; }
    public bool Icazeler { get; set; }

    public int AktivSayi => new[] { Baxis, Yeni, DuzelisEt, AktivDeaktiv, Icazeler }.Count(x => x);
    public const int UmumiSayi = 5;
}
