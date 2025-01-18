namespace DongKeJi;

/// <summary>
/// 程序配置
/// </summary>
public interface IApplicationConfig
{
    /// <summary>
    /// 程序根目录
    /// </summary>
    string DirectoryBase { get; }

    /// <summary>
    ///     数据库文件目录
    /// </summary>
    string DirectoryDatabase { get; }

    /// <summary>
    ///     缓存文件目录
    /// </summary>
    string DirectoryCache { get; }



    /// <summary>
    /// 更新版本列表网址
    /// </summary>
    string UpdateVersionListUrl { get; }

    /// <summary>
    /// 更新下载文件网址
    /// </summary>
    string UpdateDownloadHost { get; }
}