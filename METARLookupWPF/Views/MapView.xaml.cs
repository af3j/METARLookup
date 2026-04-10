using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using METARLookupWPF.Models;

namespace METARLookupWPF.Views;

public partial class MapView : UserControl
{
    private bool _initialized;

    public MapView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_initialized) return;
        _initialized = true;
        await MapWebView.EnsureCoreWebView2Async();
        MapWebView.NavigateToString(BuildMapHtml(null, null, []));
    }

    public async Task ShowAirportAsync(double? lat, double? lon, List<Metar> nearby)
    {
        if (!_initialized) return;
        await MapWebView.EnsureCoreWebView2Async();
        MapWebView.NavigateToString(BuildMapHtml(lat, lon, nearby));
    }

    private static string BuildMapHtml(double? lat, double? lon, List<Metar> nearby)
    {
        double centerLat = lat ?? 39.8283;
        double centerLon = lon ?? -98.5795;
        int zoom = lat.HasValue ? 9 : 4;
        string centerLatStr = centerLat.ToString(CultureInfo.InvariantCulture);
        string centerLonStr = centerLon.ToString(CultureInfo.InvariantCulture);

        var markers = new StringBuilder();
        if (lat.HasValue && lon.HasValue)
        {
            markers.AppendLine(
                $"addMarker({centerLatStr}, {centerLonStr}, 'primary', '');");
        }

        // Use $$""" so that {{centerLatStr}} is a C# interpolation, and {single} braces are literal JS
        return $$"""
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="utf-8"/>
              <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"/>
              <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
              <style>
                html, body, #map { margin:0; padding:0; height:100%; width:100%; }
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
                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                  attribution: '&copy; OpenStreetMap contributors',
                  maxZoom: 18
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
