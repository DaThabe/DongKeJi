using System.IO;
using System.Windows;
using System.Windows.Threading;
using DongKeJi.Deploy.Service;
using DongKeJi.Deploy.UI.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.Deploy;


public partial class App
{
    public static IHost Host { get; } = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureServices(x =>
        {
            x.AddSingleton<IConfig>(Config.Instance);
            x.AddSingleton<IPublishService, PublishService>();
            x.AddSingleton<IUpdateService, UpdateService>();
            x.AddSingleton<IDeployService, DeployService>();
            x.AddSingleton<IVersionService, VersionService>();


            x.AddSingleton<UpdateWindow>();
            x.AddSingleton<DevelopmentWindow>();
        })
        .Build();

    /// <summary>
    /// 是否是开发模式
    /// </summary>
    public static bool IsDevelopment { get; } = true;


    protected override void OnStartup(StartupEventArgs e)
    {
        Current.Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                if (!IsDevelopment)
                {
                    var win = Host.Services.GetRequiredService<UpdateWindow>();
                    Current.MainWindow = win;
                    win.Show();
                    await win.LazyInitAsync();
                }
                else
                {
                    var win = Host.Services.GetRequiredService<DevelopmentWindow>();
                    Current.MainWindow = win;
                    win.Show();
                }
            }
            catch (Exception ex)
            {
                var fileName = $"{DateTime.Now:yyyyMMdd-HHmmss}-Error.log";
                await File.WriteAllTextAsync(fileName, ex.ToString());

                MessageBox.Show(ex.Message, "发生错误");
                Shutdown(-1);
            }

        }, DispatcherPriority.Send);
    }
}