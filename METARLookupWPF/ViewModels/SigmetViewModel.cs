using CommunityToolkit.Mvvm.ComponentModel;
using METARLookupWPF.Models;

namespace METARLookupWPF.ViewModels;

public partial class SigmetViewModel : ObservableObject
{
    [ObservableProperty] private List<Sigmet> _sigmets = [];
    [ObservableProperty] private bool _hasSigmets;
    [ObservableProperty] private int _sigmetCount;

    public void Load(List<Sigmet> sigmets)
    {
        Sigmets = sigmets;
        SigmetCount = sigmets.Count;
        HasSigmets = sigmets.Count > 0;
    }
}
