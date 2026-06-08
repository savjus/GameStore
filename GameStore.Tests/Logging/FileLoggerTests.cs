using System.Globalization;
using GameStore.Logging;
using Microsoft.Extensions.Logging;

namespace GameStore.Tests.Logging;

public class FileLoggerTests
{
    [Fact]
    public void Log_WritesToDailyLogFile()
    {
        var rootPath = CreateTempRoot();
        try
        {
            var options = new FileLoggerOptions
            {
                LogDirectory = "TestLogs",
                LogFilePrefix = "app",
                ExceptionFilePrefix = "app-exceptions",
                MinimumLevel = LogLevel.Information,
            };

            var logger = new FileLogger("TestCategory", options, rootPath);

            logger.Log(LogLevel.Information, new EventId(1, "info"), "hello", null, (state, _) => state.ToString()!);

            var logFile = GetDailyFilePath(rootPath, options.LogDirectory, options.LogFilePrefix);
            var exceptionFile = GetDailyFilePath(rootPath, options.LogDirectory, options.ExceptionFilePrefix);

            Assert.True(File.Exists(logFile));
            Assert.Contains("hello", File.ReadAllText(logFile));
            Assert.False(File.Exists(exceptionFile));
        }
        finally
        {
            CleanupTempRoot(rootPath);
        }
    }

    [Fact]
    public void Log_ErrorWritesToExceptionLog()
    {
        var rootPath = CreateTempRoot();
        try
        {
            var options = new FileLoggerOptions
            {
                LogDirectory = "TestLogs",
                LogFilePrefix = "app",
                ExceptionFilePrefix = "app-exceptions",
                MinimumLevel = LogLevel.Information,
            };

            var logger = new FileLogger("TestCategory", options, rootPath);

            var exception = new InvalidOperationException("boom");
            logger.Log(LogLevel.Error, new EventId(2, "error"), "failed", exception, (state, _) => state.ToString()!);

            var logFile = GetDailyFilePath(rootPath, options.LogDirectory, options.LogFilePrefix);
            var exceptionFile = GetDailyFilePath(rootPath, options.LogDirectory, options.ExceptionFilePrefix);

            Assert.True(File.Exists(logFile));
            Assert.True(File.Exists(exceptionFile));
            Assert.Contains("failed", File.ReadAllText(logFile));
            Assert.Contains("failed", File.ReadAllText(exceptionFile));
            Assert.Contains("InvalidOperationException", File.ReadAllText(exceptionFile));
        }
        finally
        {
            CleanupTempRoot(rootPath);
        }
    }

    private static string GetDailyFilePath(string rootPath, string directory, string prefix)
    {
        var dateStamp = DateTimeOffset.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return Path.Combine(rootPath, directory, $"{prefix}-{dateStamp}.txt");
    }

    private static string CreateTempRoot()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "gamestore-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);
        return rootPath;
    }

    private static void CleanupTempRoot(string rootPath)
    {
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
    }
}
