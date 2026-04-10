using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using METARLookupWPF.Models;
using Microsoft.Web.WebView2.Core;

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
        await MapWebView.EnsureCoreWebView2Async();

        // Map the virtual hostname to the temp directory so the WebView can load map.html via HTTPS.
        MapWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            VirtualHostName, _tempMapDir, CoreWebView2HostResourceAccessKind.Allow);

        // Render with whatever airport was requested before initialisation completed.
        NavigateMap(BuildMapHtml(_pendingLat, _pendingLon, _pendingNearby));
    }

    /// <summary>
    /// Updates the map to show the given airport and nearby METAR stations.
    /// If WebView2 is not yet ready, stores the values so OnLoaded will apply them.
    /// Called from MainWindow.xaml.cs when NearbyMetars changes or the Map tab is selected.
    /// </summary>
    public async Task ShowAirportAsync(double? lat, double? lon, List<Metar> nearby)
    {
        _pendingLat = lat;
        _pendingLon = lon;
        _pendingNearby = nearby;

        // If WebView2 hasn't finished initialising, OnLoaded will pick up the pending values.
        if (!_initialized) return;
        await MapWebView.EnsureCoreWebView2Async();
        NavigateMap(BuildMapHtml(lat, lon, nearby));
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
    /// </summary>
    private static string BuildMapHtml(double? lat, double? lon, List<Metar> nearby)
    {
        // Default centre: geographic centre of the contiguous United States.
        double centerLat = lat ?? 39.8283;
        double centerLon = lon ?? -98.5795;
        int zoom = lat.HasValue ? 12 : 4;

        // InvariantCulture ensures decimal separators are '.' in the JavaScript literals,
        // regardless of the OS locale setting.
        string centerLatStr = centerLat.ToString(CultureInfo.InvariantCulture);
        string centerLonStr = centerLon.ToString(CultureInfo.InvariantCulture);

        var markers = new StringBuilder();

        // Add the primary (searched) airport marker first so it renders on top.
        if (lat.HasValue && lon.HasValue)
        {
            markers.AppendLine(
                $"addMarker({centerLatStr}, {centerLonStr}, 'primary', '');");
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
                /* Flight-category colour coding matches FAA standard colours */
                .cat-vfr  { background:#22BB45; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-mvfr { background:#2288FF; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-ifr  { background:#EE4433; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-lifr { background:#AA22AA; border:2px solid #fff; border-radius:50%; width:14px; height:14px; }
                .cat-primary { background:#FF9900; border:3px solid #fff; border-radius:50%; width:18px; height:18px; box-shadow:0 0 8px rgba(255,153,0,0.8); }
              </style>
            </head>
            <body>
              <div id="map"></div>
              <script>
                var map = L.map('map').setView([{{centerLatStr}}, {{centerLonStr}}], {{zoom}});
                L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
                  attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
                  maxZoom: 19
                }).addTo(map);

                function addMarker(lat, lon, cat, label) {
                  var icon = L.divIcon({ className: 'cat-' + cat, iconSize: [14,14] });
                  L.marker([lat, lon], {icon: icon}).addTo(map).bindPopup(label || cat);
                }

                {{markers}}
              </script>
            </body>
            </html>
            """;
    }
}
