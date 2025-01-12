using System.IO;
using System.Windows;
using System.Windows.Threading;
using DongKeJi.Core;
using DongKeJi.Module;
using DongKeJi.Work;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Launcher;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IApplication
{
    public IHost Host { get; }

    /// <summary>
    ///     程序根目录
    /// </summary>
    public string BaseDirectory { get; } 

    /// <summary>
    ///     数据库文件目录
    /// </summary>
    public string DatabaseDirectory { get; }


    public App()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .RegisterModule<CoreModule>()
            .RegisterModule<LauncherModule>()
            .RegisterModule<WorkModule>()
            .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(services =>
            {
                services.AddHostedService<HostedService>();
                services.AddSingleton<IApplication>(this);
            });

        Host = builder.Build();
        BaseDirectory = AppContext.BaseDirectory;
        DatabaseDirectory = Path.Combine(BaseDirectory, "Database");
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
    ///     Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        var snakebar = Host.Services.GetRequiredService<ISnackbarService>();
        snakebar.Show("发生错误", e.Exception.Message, ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.ErrorCircle16), TimeSpan.FromSeconds(5));
        e.Handled = true;
    }

}