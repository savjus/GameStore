using System.Collections.Concurrent;

namespace GameStore.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly FileLoggerOptions _options;
    private readonly string _rootPath;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public FileLoggerProvider(FileLoggerOptions options, string rootPath)
    {
        _options = options;
        _rootPath = rootPath;
        Directory.CreateDirectory(GetLogDirectoryPath());
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new FileLogger(name, _options, _rootPath));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }

    internal string GetLogDirectoryPath()
    {
        return Path.Combine(_rootPath, _options.LogDirectory);
    }
}
