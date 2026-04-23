using System.IO;
using Microsoft.Web.WebView2.Core;

namespace METARLookupWPF.Services;

/// <summary>
/// Provides a single shared <see cref="CoreWebView2Environment"/> for the whole process.
///
/// WebView2 requires that every control in the same process which uses the same user-data
/// folder shares the *exact same* CoreWebView2Environment instance — passing two different
/// objects (even to different folder paths) that collide at the browser-process level causes:
///   "WebView2 was already initialized with a different CoreWebView2Environment."
///
/// The environment is created lazily on the first call and then returned from cache on
/// every subsequent call, so MapView and ChartsView both end up with the same object.
/// Thread-safe via SemaphoreSlim.
/// </summary>
internal static class WebView2EnvironmentService
{
    private static CoreWebView2Environment? _instance;
    private static readonly SemaphoreSlim   _lock = new(1, 1);

    /// <summary>
    /// Returns the shared environment, creating it on the first call.
    /// User data is stored in %LocalAppData%\METARLookup\WebView2 so the app works
    /// when installed under Program Files (which is read-only for normal users).
    /// </summary>
    public static async Task<CoreWebView2Environment> GetAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_instance is null)
            {
                var userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "METARLookup", "WebView2");

                _instance = await CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: null,
                    userDataFolder:          userDataFolder);
            }
            return _instance;
        }
        finally
        {
            _lock.Release();
        }
    }
}
