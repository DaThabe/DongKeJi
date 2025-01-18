using System.IO;
using System.Windows;
using System.Windows.Threading;
using DongKeJi.Core;
using DongKeJi.Launcher.Service;
using DongKeJi.Module;
using DongKeJi.Tool;
using DongKeJi.WebView;
using DongKeJi.Work;
using Microsoft.Extensions.Configuration;
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
    public string DirectoryBase { get; }
    public string DirectoryDatabase { get; }
    public string DirectoryCache { get; }

    public string UpdateVersionListUrl =>
        _configuration[nameof(UpdateVersionListUrl)] ?? 
        throw new ArgumentNullException(nameof(UpdateVersionListUrl), "无法获取更新版本列表网址, 因为未读取到配置");

    public string UpdateDownloadHost =>
        _configuration[nameof(UpdateDownloadHost)] ??
        throw new ArgumentNullException(nameof(UpdateDownloadHost), "无法获取更新下载地址, 因为未读取到配置");


    public string Title { get; }
    public Version Version { get;  }
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
        Version = new Version(0, 0, 1);
        Title = $"懂科技 - {Version}";

        DirectoryBase = AppContext.BaseDirectory;
        DirectoryCache = Path.Combine(AppContext.BaseDirectory, "Cache");
        DirectoryDatabase = Path.Combine(AppContext.BaseDirectory, "Database");

        var builder = Host.CreateDefaultBuilder()
            .RegisterModule<CoreModule>()
            .RegisterModule<WorkModule>()
            .RegisterModule<ToolModule>()
            .RegisterModule<WebViewModule>()
            .RegisterModule<LauncherModule>()
            .ConfigureAppConfiguration(x =>
            {
                x.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<HostedService>();
                services.AddSingleton<IApplication>(this);
            });
            

        _host = builder.Build();

        _coreConfig = _host.Services.GetRequiredService<ICoreConfig>();
        _configuration = _host.Services.GetRequiredService<IConfiguration>();
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

    /// <summary>
    /// 
    /// </summary>
    private readonly ICoreConfig _coreConfig;

    private readonly IConfiguration _configuration;
}


file class ApplicationConfig : IApplicationConfig
{
    public required string DirectoryBase { get; init; }
    public required string DirectoryDatabase { get; init; }
    public required string DirectoryCache { get; init; }

    public required string UpdateVersionListUrl { get; init; }
    public required string UpdateDownloadHost { get; init; }
}