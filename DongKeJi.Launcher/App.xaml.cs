using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Wpf.Ui;
using System.Windows.Threading;
using Wpf.Ui.Controls;
using DongKeJi.Launcher.Service;
using DongKeJi.Common.Module;
using System.Text;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using DongKeJi.Work;


namespace DongKeJi.Launcher;


/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IHost Host { get; }

    public App()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .RegisterModule<DongKeJiModule>()
            .RegisterModule<PerformanceRecordModule>()
            .ConfigureLogging(x=>x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(services  =>
            {
                services.AddHostedService<HostedService>();
                services.AddSingleton<Application>(this);
            });

        Host = builder.Build();
    }


    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await Host.StartAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Host.StopAsync();
        base.OnExit(e);
    }


    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        var snakebar = Host.Services.GetRequiredService<ISnackbarService>();
        snakebar.Show("发生错误", e.Exception.Message, ControlAppearance.Danger, new SymbolIcon(SymbolRegular.ErrorCircle16), TimeSpan.FromSeconds(5));
        e.Handled = true;
    }
}
