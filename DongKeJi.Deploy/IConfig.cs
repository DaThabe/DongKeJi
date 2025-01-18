namespace DongKeJi.Deploy;

public interface IConfig
{
    /// <summary>
    /// 启动器文件名称
    /// </summary>
    public string LauncherFileName { get; }

    /// <summary>
    /// 版本列表网址
    /// </summary>
    public string VersionListUrl { get; }

    /// <summary>
    /// 下载文件网址
    /// </summary>
    public string DownloadHost { get; }
}


internal class Config : IConfig
{
    public static Config Instance { get; } = new()
    {
        LauncherFileName = "DongKeJi.Launcher.exe",
        VersionListUrl = "https://gitee.com/dongkeji-cloud/release/raw/master/VersionList.json",
        DownloadHost = "https://gitee.com/dongkeji-cloud/release/raw/master/Version/"
    };

    private Config()
    {
    }



    public required string LauncherFileName { get; init; }
    public required string VersionListUrl { get; init; }
    public required string DownloadHost { get; init; }
}