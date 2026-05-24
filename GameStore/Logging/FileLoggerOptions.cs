namespace GameStore.Logging;

public sealed class FileLoggerOptions
{
    public string LogDirectory { get; set; } = "Logs";

    public string LogFilePrefix { get; set; } = "gamestore";

    public string ExceptionFilePrefix { get; set; } = "gamestore-exceptions";

    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
}
