using System.Windows;
using DongKeJi.Deploy.Model;
using DongKeJi.Deploy.Service;

namespace DongKeJi.Deploy;

public partial class App : Application
{
    public App()
    {
        Fuck();
    }



    public async Task Fuck()
    {
        try
        {
            Updater updater = new();
            var newVersion = await updater.GetLatestVersionAsync();

            await updater.DownloadVersionFileAsync(newVersion, "Download", new Progress<(string fileName, double progress)>(x =>
            {
                string fuck = $"{x.fileName} - {x.progress}";
            }));

            var shit = 1;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        

        //Publisher publisher = new();
        //const string folder = @"D:\Src\Data\Code\懂科技\code-hosting\V2\DongKeJi\DongKeJi.Launcher\bin\Release\net8.0-windows\publish\win-x64";

        //var versionInfo = await publisher.CreateByFolderAsync(folder, new Version(0, 0, 1), "");

        //var json = versionInfo.ToJson();

        //Deployer deployer = new Deployer();
        //await deployer.ValidateByFolderAsync(versionInfo, folder);
    }
}