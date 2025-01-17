using System.IO;
using DongKeJi.Deploy.Extensions;
using DongKeJi.Deploy.Model;

namespace DongKeJi.Deploy.Service;

public interface IPublisher
{
    /// <summary>
    /// 从文件发布
    /// </summary>
    /// <param name="version">版本</param>
    /// <param name="intro">价绍</param>
    /// <param name="isMandatory">是否强制更新</param>
    /// <param name="updateType">更新类型</param>
    /// <param name="files">所有更新的文件</param>
    /// <returns></returns>
    ValueTask<VersionInfo> CreateByFilePathsAsync(
        IEnumerable<string> files,
        Version version,
        string intro,
        bool isMandatory = false,
        UpdateTypeEnum updateType = UpdateTypeEnum.Patch);
}

public static class PublisherExtensions
{
    /// <summary>
    /// 从目录创建
    /// </summary>
    /// <param name="publisher"></param>
    /// <param name="version">版本</param>
    /// <param name="intro">价绍</param>
    /// <param name="isMandatory">是否强制更新</param>
    /// <param name="updateType">更新类型</param>
    /// <param name="folder">所有更新文件所在目录</param>
    /// <returns></returns>
    public static ValueTask<VersionInfo> CreateByFolderAsync(
        this IPublisher publisher,
        string folder,
        Version version,
        string intro,
        bool isMandatory = false,
        UpdateTypeEnum updateType = UpdateTypeEnum.Patch)
    {
        var files = Directory.EnumerateFiles(folder);
        return publisher.CreateByFilePathsAsync(files, version, intro, isMandatory, updateType);
    }
}

public class Publisher : IPublisher
{
    public async ValueTask<VersionInfo> CreateByFilePathsAsync(
        IEnumerable<string> files,
        Version version,
        string intro,
        bool isMandatory,
        UpdateTypeEnum updateType)
    {
        List<VersionFileInfo> fileInfos = [];

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);

            var versionFileInfo = new VersionFileInfo
            {
                Md5 = await HashExtensions.CalcFileMd5Async(file),
                Length = new FileInfo(file).Length,
                Name = fileName,
                DownloadUrl = fileName
            };

            fileInfos.Add(versionFileInfo);
        }

        VersionInfo versionInfo = new()
        {
            Version = version,
            Intro = intro,
            IsMandatory = isMandatory,
            UpdateType = updateType,
            Files = fileInfos.ToArray(),
            UpdateSize = fileInfos.Sum(x => x.Length)
        };

        return versionInfo;
    }
}