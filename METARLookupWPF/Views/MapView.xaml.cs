using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using METARLookupWPF.Models;
using METARLookupWPF.Services;
using Microsoft.Web.WebView2.Core;
using ModernWpf;

namespace METARLookupWPF.Views;

/// <summary>
/// UserControl that hosts a Leaflet.js interactive map inside a WebView2 control.
/// The map is rendered as a self-contained HTML file written to a temp directory and
/// served via a WebView2 virtual host mapping so the browser security sandbox is satisfied.
/// Flight-category markers are colour-coded: green=VFR, blue=MVFR, red=IFR, purple=LIFR,
/// orange=primary (the searched airport).
/// </summary>
public partial class MapView : UserControl
{
    // Guards against re-initialising WebView2 if the control is unloaded and reloaded.
    private bool _initialized;

    // A stable temp directory for the generated HTML file.
    // Using a named subfolder avoids collisions with other apps in the system temp dir.
    private static readonly string _tempMapDir = Path.Combine(Path.GetTempPath(), "MetarLookupWPF");
    private static readonly string _tempMapPath = Path.Combine(_tempMapDir, "map.html");

    // Virtual host name used by WebView2 to serve local files as if from an HTTPS origin.
    // This avoids mixed-content and CORS issues when loading Leaflet from a CDN.
    private const string VirtualHostName = "metar.local";

    // Stores the last requested airport coordinates so OnLoaded can apply them
    // if ShowAirportAsync was called before WebView2 finished initialising.
    private double? _pendingLat;
    private double? _pendingLon;
    private List<Metar> _pendingNearby = [];
    private string? _pendingRadarUrl;
    private string? _pendingPrimaryIcao;

    // Dedicated client for the one-off RainViewer metadata request.
    private static readonly HttpClient _radarHttp = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    /// <summary>
    /// Raised when the user clicks "Look Up" on a nearby station's popup.
    /// The string argument is the station's ICAO code.
    /// </summary>
    public event Action<string>? StationSelected;

    public MapView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Initialises WebView2 the first time the control becomes visible.
    /// Sets up the virtual host mapping so the temp HTML file can be loaded as HTTPS.
    /// Uses pending values so any early ShowAirportAsync call is not lost.
    /// </summary>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_initialized) return;
        _initialized = true;

        //#if DEBUG
        //        // Remove after Sentry is verified.
        //        static void ThrowTestMapError() =>
        //            throw new InvalidOperationException("Sentry test — deliberate map initialisation error.");
        //        ThrowTestMapError();
        //#endif

        try
        {
            // All WebView2 controls in the process must share one CoreWebView2Environment instance.
            // WebView2EnvironmentService creates it once (pointed at %LocalAppData%\METARLookup\WebView2)
            // and returns the same object on every subsequent call.
            await MapWebView.EnsureCoreWebView2Async(await WebView2EnvironmentService.GetAsync());

            // Must exist before SetVirtualHostNameToFolderMapping is called — it does not create it.
            Directory.CreateDirectory(_tempMapDir);

            // Map the virtual hostname to the temp directory so the WebView can load map.html via HTTPS.
            MapWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                VirtualHostName, _tempMapDir, CoreWebView2HostResourceAccessKind.Allow);

            // When JS calls window.chrome.webview.postMessage(icao), raise StationSelected.
            MapWebView.CoreWebView2.WebMessageReceived += (_, args) =>
            {
                var icao = args.TryGetWebMessageAsString();
                if (!string.IsNullOrWhiteSpace(icao))
                    StationSelected?.Invoke(icao);
            };

            // Render with whatever airport was requested before initialisation completed.
            NavigateMap(BuildMapHtml(_pendingLat, _pendingLon, _pendingNearby, _pendingRadarUrl, _pendingPrimaryIcao));
        }
        catch (Exception ex)
        {
            try
            {
                MapWebView.Visibility = Visibility.Collapsed;
                MapErrorPanel.Visibility = Visibility.Visible;
                MapErrorText.Text = DescribeWebView2Error(ex);
            }
            catch { /* innermost safety net — swallow so we never crash the app from here */ }
        }
    }

    private static string DescribeWebView2Error(Exception ex)
    {
        var cause = unchecked((uint)ex.HResult) switch
        {
            0x80070005 => "Access was denied to the map's data folder. Try running the app as administrator.",
            0x80070002 => "The WebView2 Runtime could not be found on this machine.",
            0x80070003 => "The map's data folder path could not be found.",
            0x80070570 => "The WebView2 user data folder appears to be corrupted.",
            0x8007007E => "A required WebView2 component (DLL) is missing.",
            0x800700B7 => "A WebView2 profile conflict was detected — another instance may be running.",
            0x8000FFFF => "An unexpected internal error occurred in the WebView2 component.",
            _          => null
        };

        var lines = new System.Text.StringBuilder();
        lines.AppendLine(cause ?? $"The map component failed to initialize.");
        lines.AppendLine();

        if (cause == null || unchecked((uint)ex.HResult) == 0x80070002)
            lines.AppendLine("• Try reinstalling the WebView2 Runtime from Microsoft (microsoft.com/edge/webview2)");

        if (unchecked((uint)ex.HResult) == 0x80070570)
        {
            var dataFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "METARLookup", "WebView2");
            lines.AppendLine($"• Try deleting the folder: {dataFolder}");
        }

        lines.AppendLine("• Use the 'Report Bug' button to send a report if the problem persists.");
        lines.AppendLine();
        lines.Append($"Error code: 0x{ex.HResult:X8}");

        return lines.ToString();
    }

    /// <summary>
    /// Switches the map tile layer and label colours via JavaScript without reloading the page.
    /// This is instant — no page blank, no CDN re-fetch, no tile reload.
    /// Safe to call even if no airport has been loaded yet (the JS guard handles it).
    /// </summary>
    public async Task SetThemeAsync(bool isDark)
    {
        if (!_initialized) return;
        await MapWebView.EnsureCoreWebView2Async();
        // setMapTheme is defined in the generated HTML; the guard is a safety net for
        // the rare case where the page hasn't finished loading yet.
        string script = isDark ? "if(window.setMapTheme)setMapTheme(true);"
                                : "if(window.setMapTheme)setMapTheme(false);";
        await MapWebView.ExecuteScriptAsync(script);
    }

    /// <summary>
    /// Updates the map to show the given airport and nearby METAR stations.
    /// If WebView2 is not yet ready, stores the values so OnLoaded will apply them.
    /// Called from MainWindow.xaml.cs when NearbyMetars changes or the Map tab is selected.
    /// </summary>
    public async Task ShowAirportAsync(double? lat, double? lon, List<Metar> nearby, string? primaryIcao = null)
    {
        _pendingLat = lat;
        _pendingLon = lon;
        _pendingNearby = nearby;
        _pendingPrimaryIcao = primaryIcao;
        _pendingRadarUrl = await FetchRadarTileUrlAsync();

        // If WebView2 hasn't finished initialising, OnLoaded will pick up the pending values.
        if (!_initialized) return;
        await MapWebView.EnsureCoreWebView2Async();
        NavigateMap(BuildMapHtml(lat, lon, nearby, _pendingRadarUrl, primaryIcao));
    }

    /// <summary>
    /// Fetches the latest RainViewer radar frame path from their metadata API and returns
    /// a ready-to-use Leaflet tile URL. Returns null if the request fails or times out,
    /// in which case the map renders without the radar overlay.
    /// Fetching from C# avoids browser-sandbox restrictions that can silently block
    /// fetch() calls from WebView2 virtual-host pages.
    /// </summary>
    private static async Task<string?> FetchRadarTileUrlAsync()
    {
        try
        {
            var json = await _radarHttp.GetStringAsync("https://api.rainviewer.com/public/weather-maps.json");
            using var doc = JsonDocument.Parse(json);
            var past = doc.RootElement.GetProperty("radar").GetProperty("past");
            if (past.GetArrayLength() == 0) return null;
            var path = past[past.GetArrayLength() - 1].GetProperty("path").GetString();
            // {z}/{x}/{y} are Leaflet tile template tokens — curly braces are literal here.
            return path != null
                ? $"https://tilecache.rainviewer.com{path}/512/{{z}}/{{x}}/{{y}}/2/1_1.png"
                : null;
        }
        catch { return null; }
    }

    /// <summary>
    /// Writes the HTML string to the temp file and navigates WebView2 to the virtual HTTPS URL.
    /// Writing to a file and navigating is required because WebView2's NavigateToString
    /// cannot load external resources (Leaflet CSS/JS from CDN).
    /// </summary>
    private void NavigateMap(string html)
    {
        Directory.CreateDirectory(_tempMapDir);
        File.WriteAllText(_tempMapPath, html, Encoding.UTF8);
        MapWebView.CoreWebView2.Navigate($"https://{VirtualHostName}/map.html");
    }

    /// <summary>
    /// Generates a complete Leaflet HTML page as a string.
    /// When no airport coordinates are provided, centres the map over the continental US at zoom 4.
    /// Nearby METARs are emitted as JavaScript addMarker() calls colour-coded by flight category.
    /// Automatically uses the CartoDB Dark Matter tile layer when the WPF application theme is dark.
    /// </summary>
    private static string BuildMapHtml(double? lat, double? lon, List<Metar> nearby, string? radarTileUrl, string? primaryIcao)
    {
        // Default centre: geographic centre of the contiguous United States.
        double centerLat = lat ?? 39.8283;
        double centerLon = lon ?? -98.5795;
        int zoom = lat.HasValue ? 10 : 4;

        // InvariantCulture ensures decimal separators are '.' in the JavaScript literals,
        // regardless of the OS locale setting.
        string centerLatStr = centerLat.ToString(CultureInfo.InvariantCulture);
        string centerLonStr = centerLon.ToString(CultureInfo.InvariantCulture);

        // Choose tile layer and label colours based on the current WPF application theme.
        bool isDark = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
        string tileUrl = isDark
            ? "https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
            : "https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png";
        string labelColor      = isDark ? "#fff" : "#111";
        string labelTextShadow = isDark
            ? "0 0 3px #000, 0 0 3px #000"
            : "0 0 3px #fff, 0 0 3px #fff";

        var markers = new StringBuilder();

        // Build the radar layer snippet here so it can be injected into the raw string below
        // without conflicting with the $$ interpolation brace rules.
        string radarJs = radarTileUrl != null
            ? $"L.tileLayer('{radarTileUrl}', {{ opacity: 0.5, maxNativeZoom: 6, maxZoom: 19, attribution: 'RainViewer' }}).addTo(map);"
            : "// radar unavailable";

        // Nearby stations first (drawn below the primary marker).
        // Skip the primary airport itself to avoid a duplicate dot underneath the orange marker.
        foreach (var m in nearby.Where(m =>
            m.Latitude.HasValue && m.Longitude.HasValue &&
            m.StationId != primaryIcao))
        {
            var cat = (m.FlightCategory ?? string.Empty).ToLowerInvariant() switch
            {
                "vfr"  => "vfr",
                "mvfr" => "mvfr",
                "ifr"  => "ifr",
                "lifr" => "lifr",
                _      => "mvfr"
            };
            var mLat   = m.Latitude!.Value.ToString(CultureInfo.InvariantCulture);
            var mLon   = m.Longitude!.Value.ToString(CultureInfo.InvariantCulture);
            var icao   = (m.StationId ?? string.Empty).Replace("'", "");
            var label  = $"{icao} — {(m.FlightCategory ?? "?")}";
            markers.AppendLine($"addMarker({mLat}, {mLon}, '{cat}', '{icao}', '{label}');");
        }

        // Primary airport last so its orange marker renders on top.
        if (lat.HasValue && lon.HasValue)
        {
            markers.AppendLine(
                $"addMarker({centerLatStr}, {centerLonStr}, 'primary', '', '{primaryIcao ?? ""}');");
        }

        // $$""" is a raw interpolated string; {{...}} is C# interpolation while {single braces} are literal JS.
        // The nearby METAR marker block is injected as {{markers}} below.
        return $$"""
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="utf-8"/>
              <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/leaflet@1.9.4/dist/leaflet.css"/>
              <script src="https://cdn.jsdelivr.net/npm/leaflet@1.9.4/dist/leaflet.js"></script>
              <style>
                html, body, #map { margin:0; padding:0; height:100%; width:100%; }
                :root { --label-color: {{labelColor}}; --label-shadow: {{labelTextShadow}}; }
                /* Flight-category colour coding matches FAA standard colours */
                .cat-vfr  { background:#22BB45; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-mvfr { background:#2288FF; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-ifr  { background:#EE4433; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-lifr { background:#AA22AA; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-primary { background:#FF9900; border:3px solid #fff; border-radius:50%; width:18px; height:18px; box-shadow:0 0 8px rgba(255,153,0,0.8); }
                .icao-label { background:transparent; border:none; box-shadow:none; font-size:10px; font-weight:bold; color:var(--label-color); text-shadow:var(--label-shadow); white-space:nowrap; }
              </style>
            </head>
            <body>
              <div id="map"></div>
              <script>
                var map = L.map('map').setView([{{centerLatStr}}, {{centerLonStr}}], {{zoom}});
                window._baseTileLayer = L.tileLayer('{{tileUrl}}', {
                  attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
                  maxZoom: 19
                }).addTo(map);
                /* Called by C# via ExecuteScriptAsync to switch themes without a page reload */
                window.setMapTheme = function(isDark) {
                  var url = isDark
                    ? 'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png'
                    : 'https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png';
                  window._baseTileLayer.setUrl(url);
                  var r = document.documentElement;
                  r.style.setProperty('--label-color',  isDark ? '#fff' : '#111');
                  r.style.setProperty('--label-shadow', isDark ? '0 0 3px #000, 0 0 3px #000'
                                                               : '0 0 3px #fff, 0 0 3px #fff');
                };

                {{radarJs}}

                function selectStation(icao) {
                  window.chrome.webview.postMessage(icao);
                }

                function addMarker(lat, lon, cat, icao, label) {
                  var size = cat === 'primary' ? [18,18] : [14,14];
                  var icon = L.divIcon({ className: 'cat-' + cat, iconSize: size });
                  var marker = L.marker([lat, lon], {icon: icon}).addTo(map);
                  if (icao) {
                    marker.bindTooltip(icao, { permanent: true, direction: 'top', className: 'icao-label', offset: [0, -4] });
                    var popupHtml = '<b>' + (label || icao) + '</b>' +
                      (cat !== 'primary'
                        ? '<br><a href="#" onclick="selectStation(\'' + icao + '\');return false;" style="font-size:11px;">Look Up</a>'
                        : '');
                    marker.bindPopup(popupHtml);
                  }
                }

                {{markers}}
              </script>
            </body>
            </html>
            """;
    }
}
