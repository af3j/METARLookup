using CommunityToolkit.Mvvm.ComponentModel;
using METARLookupWPF.Models;

namespace METARLookupWPF.ViewModels;

/// <summary>
/// View-model for the SIGMETs/AIRMETs tab. Holds the full list of active advisories
/// and exposes a count that the tab header uses to show a red badge when advisories are active.
/// </summary>
public partial class SigmetViewModel : ObservableObject
{
    /// <summary>All currently active SIGMET and AIRMET advisories worldwide.</summary>
    [ObservableProperty] private List<Sigmet> _sigmets = [];

    /// <summary>True when at least one advisory is active; controls placeholder visibility in the view.</summary>
    [ObservableProperty] private bool _hasSigmets;

    /// <summary>
    /// Number of active advisories. Bound to the red badge on the SIGMETs tab header
    /// so pilots immediately notice when advisories are in effect.
    /// </summary>
    [ObservableProperty] private int _sigmetCount;

    /// <summary>
    /// Replaces the current advisory list with fresh data from the API.
    /// Called by <see cref="MainViewModel.FetchAllAsync"/> on every lookup.
    /// </summary>
    public void Load(List<Sigmet> sigmets)
    {
        Sigmets = sigmets;
        SigmetCount = sigmets.Count;
        HasSigmets = sigmets.Count > 0;
    }
}
