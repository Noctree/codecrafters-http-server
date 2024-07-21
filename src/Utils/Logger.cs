using System.Globalization;
using System.Runtime.CompilerServices;

namespace codecrafters_http_server.Utils;

public static class Logger
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static void Log(LogLevel level, string message,
        [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "")
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        var threadId = Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture);
        var className = Path.GetFileNameWithoutExtension(sourceFilePath);

        var logEntry = $"[{timestamp}] [{level}] [{className}.{memberName}] (Thread: {threadId}): {message}";

        var originalColor = Console.ForegroundColor;
        SetConsoleColor(level);

        Console.WriteLine(logEntry);

        Console.ForegroundColor = originalColor;
    }

    private static void SetConsoleColor(LogLevel level)
    {
        Console.ForegroundColor = level switch
        {
            LogLevel.Debug => ConsoleColor.Cyan,
            LogLevel.Info => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => Console.ForegroundColor
        };
    }

    // Convenience methods for each log level
    public static void Debug(string message, [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "")
        => Log(LogLevel.Debug, message, sourceFilePath, memberName);

    public static void Info(string message, [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "")
        => Log(LogLevel.Info, message, sourceFilePath, memberName);

    public static void Warning(string message, [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "")
        => Log(LogLevel.Warning, message, sourceFilePath, memberName);

    public static void Error(string message, [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "")
        => Log(LogLevel.Error, message, sourceFilePath, memberName);
}