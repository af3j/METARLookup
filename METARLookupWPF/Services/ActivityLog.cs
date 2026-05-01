namespace METARLookupWPF.Services;

/// <summary>
/// Thread-safe circular buffer of recent user actions. Included in crash/bug reports
/// to give context about what the user was doing before the problem occurred.
/// </summary>
internal static class ActivityLog
{
    private static readonly Queue<string> _entries = new();
    private static readonly object _lock = new();
    private const int MaxEntries = 15;

    public static void Record(string activity)
    {
        lock (_lock)
        {
            _entries.Enqueue($"{DateTime.UtcNow:HH:mm:ss} UTC — {activity}");
            while (_entries.Count > MaxEntries)
                _entries.Dequeue();
        }
    }

    public static IReadOnlyList<string> GetEntries()
    {
        lock (_lock)
            return _entries.ToArray();
    }
}
