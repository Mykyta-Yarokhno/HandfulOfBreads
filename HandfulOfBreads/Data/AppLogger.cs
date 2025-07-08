using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public static class AppLogger
{
    private static ILogger? _logger;

    public static void Initialize(ILogger logger)
    {
        _logger = logger;
    }

    public static void Info(string message, [CallerMemberName] string caller = "")
        => LogInfo($"🟢 [{caller}] {message}");

    public static void Warning(string message, [CallerMemberName] string caller = "")
        => LogWarning($"🟡 [{caller}] {message}");

    public static void Error(string message, [CallerMemberName] string caller = "")
        => LogError($"🔴 [{caller}] {message}");

    private static void LogInfo(string message)
    {
        Debug.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
        _logger?.LogInformation(message);
    }

    private static void LogWarning(string message)
    {
        Debug.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
        _logger?.LogWarning(message);
    }

    private static void LogError(string message)
    {
        Debug.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
        _logger?.LogError(message);
    }
}
