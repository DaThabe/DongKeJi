using System.IO;
using DongKeJi.Deploy.Extensions;
using DongKeJi.Deploy.Model;

namespace DongKeJi.Deploy.Service;

public interface IPublishService
{
    /// <summary>
    /// 从文件发布
    /// </summary>
    /// <param name="files">所有更新的文件</param>
    /// <returns></returns>
    ValueTask<VersionItem> CreateByFilePathsAsync(IEnumerable<string> files);
}

public static class PublishExtensions
{
    /// <summary>
    /// 从目录创建
    /// </summary>
    /// <param name="iPublishService"></param>
    /// <param name="folder">所有更新文件所在目录</param>
    /// <returns></returns>
    public static ValueTask<VersionItem> CreateByFolderAsync(
        this IPublishService iPublishService,
        string folder)
    {
        var files = Directory.EnumerateFiles(folder);
        return iPublishService.CreateByFilePathsAsync(files);
    }
}

internal class PublishService(
    IConfig config,
    IVersionService versionService) : IPublishService
{
    public async ValueTask<VersionItem> CreateByFilePathsAsync(IEnumerable<string> files)
    {
        //所有文件
        List<VersionFileItem> fileInfos = [];
        string? launcherFilePath = null;

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            var versionFileInfo = new VersionFileItem
            {
                Md5 = await HashExtensions.CalcFileMd5Async(file),
                Length = new FileInfo(file).Length,
                Name = fileName,
                DownloadUrl = fileName
            };

            fileInfos.Add(versionFileInfo);

            if (versionFileInfo.Name.Contains(config.LauncherFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                launcherFilePath = file;
            }
        }

        //启动器路径
        ArgumentException.ThrowIfNullOrWhiteSpace(launcherFilePath);

        //版本元素
        VersionItem versionItem = new()
        {
            Version = versionService.GetLauncherVersion(launcherFilePath),
            Files = fileInfos.ToArray(),
            UpdateSize = fileInfos.Sum(x => x.Length)
        };

        return versionItem;
    }
}