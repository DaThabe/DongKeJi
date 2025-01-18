using System.IO;
using System.Windows;
using System.Windows.Threading;
using DongKeJi.Deploy.Extensions;
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


            x.AddSingleton<MainFrame>();
        })
        .Build();

    protected override void OnStartup(StartupEventArgs e)
    {
        Current.Dispatcher.InvokeAsync(async () =>
        {
            var publish = Host.Services.GetRequiredService<IPublishService>();
            var versionItems = await publish.CreateByFolderAsync(
                @"D:\Src\Data\Code\懂科技\开发\DongKeJi\DongKeJi.Launcher\bin\Release\net8.0-windows\publish\win-x64\懂科技-v0.0.1-beta");

            var fuck = versionItems.ToJson();


            try
            {
                var mainFrame = Host.Services.GetRequiredService<MainFrame>();
                Current.MainWindow = mainFrame;
                mainFrame.Show();

                await mainFrame.LazyInitAsync();
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