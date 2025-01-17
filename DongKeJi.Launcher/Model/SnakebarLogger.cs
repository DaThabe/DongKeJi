using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Launcher.Model;

internal class SnakebarLoggerProvider(LogLevel enableLevel, ISnackbarService snackbarService) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new SnakebarLogger(enableLevel, snackbarService, categoryName);
    }

    public void Dispose()
    {
    }
}

public class SnakebarLogger(LogLevel enableLevel, ISnackbarService snackbarService, string categoryName) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= enableLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var title = $"{DateTime.Now}";
        var message = formatter(state, exception);

        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                snackbarService.Show(title, message, ControlAppearance.Secondary, new SymbolIcon(SymbolRegular.Bug24),
                    TimeSpan.FromSeconds(2));
                break;
            case LogLevel.Information:
                snackbarService.Show(title, message, ControlAppearance.Info, new SymbolIcon(SymbolRegular.Info24),
                    TimeSpan.FromSeconds(1));
                break;
            case LogLevel.Warning:
                snackbarService.Show(title, message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Warning24),
                    TimeSpan.FromSeconds(3));
                break;
            case LogLevel.Error:
                snackbarService.Show(title, message, ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.ErrorCircle24), TimeSpan.FromSeconds(5));
                break;
            case LogLevel.Critical:
                snackbarService.Show(title, message, ControlAppearance.Dark,
                    new SymbolIcon(SymbolRegular.ClosedCaption24), TimeSpan.FromSeconds(10));
                break;
            case LogLevel.None:
            default: break;
        }
    }
}