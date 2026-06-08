using System.Globalization;
using System.Text;

namespace GameStore.Logging;

public sealed class FileLogger : ILogger
{
    private static readonly object FileLock = new();
    private readonly string _category;
    private readonly FileLoggerOptions _options;
    private readonly string _rootPath;

    public FileLogger(string category, FileLoggerOptions options, string rootPath)
    {
        _category = category;
        _options = options;
        _rootPath = rootPath;
    }

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _options.MinimumLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        var line = $"{timestamp} [{logLevel}] {_category}: {message}";

        if (exception != null)
        {
            line = $"{line}{Environment.NewLine}Exception: {exception}";
        }

        AppendLine(GetLogFilePath(_options.LogFilePrefix), line);

        if (logLevel >= LogLevel.Error)
        {
            AppendLine(GetLogFilePath(_options.ExceptionFilePrefix), line);
        }
    }

    private string GetLogFilePath(string filePrefix)
    {
        var directory = Path.Combine(_rootPath, _options.LogDirectory);
        var fileName = $"{filePrefix}-{DateTimeOffset.Now:yyyy-MM-dd}.txt";
        return Path.Combine(directory, fileName);
    }

    private static void AppendLine(string path, string content)
    {
        lock (FileLock)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.AppendAllText(path, content + Environment.NewLine, Encoding.UTF8);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
