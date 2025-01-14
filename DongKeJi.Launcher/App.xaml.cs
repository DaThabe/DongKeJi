using System.IO;
using System.Windows;
using System.Windows.Threading;
using DongKeJi.Core;
using DongKeJi.Launcher.Service;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using DongKeJi.Work;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DongKeJi.Launcher;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : IApplication
{
    /// <summary>
    ///     程序根目录
    /// </summary>
    public string BaseDirectory { get; } 

    /// <summary>
    ///     数据库文件目录
    /// </summary>
    public string DatabaseDirectory { get; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 版本
    /// </summary>
    public Version Version { get;  }

    /// <summary>
    /// 主题
    /// </summary>
    public ApplicationTheme Theme
    {
        get => ApplicationThemeManager.GetAppTheme();
        set
        {
            ApplicationThemeManager.Apply(value);
            _coreConfig.ApplicationTheme.SetAsync(value);
        }
    }


    public App()
    {
        BaseDirectory = AppContext.BaseDirectory;
        DatabaseDirectory = Path.Combine(BaseDirectory, "Database");
        Version = new Version(0, 0, 1);
        Title = $"懂科技 - {Version}";


        var builder = Host.CreateDefaultBuilder()
            .RegisterModule<CoreModule>()
            .RegisterModule<WorkModule>()
            .RegisterModule<LauncherModule>()
            .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(services =>
            {
                services.AddHostedService<HostedService>();
                services.AddSingleton<IApplication>(this);
            });

        _host = builder.Build();
        _coreConfig = _host.Services.GetRequiredService<ICoreConfig>();
    }



    #region --事件处理--

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await _host.StartAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        base.OnExit(e);
    }

    /// <summary>
    ///     Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        var snakebar = _host.Services.GetRequiredService<ISnackbarService>();
        snakebar.Show("发生错误", e.Exception.Message, ControlAppearance.Danger,
            new SymbolIcon(SymbolRegular.ErrorCircle16), TimeSpan.FromSeconds(5));
        e.Handled = true;
    }

    #endregion


    /// <summary>
    /// Host
    /// </summary>
    private readonly IHost _host;

    private readonly ICoreConfig _coreConfig;
}