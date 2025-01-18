using System.Data;
using DongKeJi.Deploy.Model;
using Flurl.Http;
using System.IO;
using System.Runtime.Loader;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DongKeJi.Deploy.Service;

public interface IUpdateService
{
    /// <summary>
    /// 更新最新版本
    /// </summary>
    /// <returns></returns>
    ValueTask UpdateAsync(CancellationToken cancellation = default);
}


public class UpdateService(
    IConfig config,
    IDeployService deployService,
    IVersionService versionService) : IUpdateService
{
    

    public async ValueTask UpdateAsync(CancellationToken cancellation = default)
    {
        //所有版本
        var versionList = await GetVersionListAsync(cancellation);

        //本地版本
        var localVersion = versionService.GetLocalLauncherVersion();
        //最新版本
        var latestVersion = versionList.LatestVersion;

        //已经是最新版本
        if (localVersion >= latestVersion)
        {
            //TODO: 大于等于最新版, 无需更新
            return;
        }

        //最新版数据不存在
        var latestVersionItem = versionList.Versions.FirstOrDefault(x => x.Version == latestVersion);
        if (latestVersionItem == null)
        {
            throw new VersionNotFoundException($"版本信息不存在: {latestVersion}");
        }

        var files = await DownloadVersionFileAsync(latestVersionItem, "Update", cancellation: cancellation);

        //测试
        await deployService.ValidateByFilePathsAsync(latestVersionItem, files, cancellation);
    }



    /// <summary>
    /// 获取版本列表
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    private async ValueTask<VersionList> GetVersionListAsync(
        CancellationToken cancellation = default)
    {
        return await config.VersionListUrl.GetJsonAsync<VersionList>(cancellationToken: cancellation);
    }


    /// <summary>
    /// 下载版本文件
    /// </summary>
    /// <param name="version"></param>
    /// <param name="saveFolder"></param>
    /// <param name="progress"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    private async ValueTask<List<string>> DownloadVersionFileAsync(
        VersionItem version, 
        string saveFolder, 
        IProgress<(string fileName, double progress)>? progress = null, 
        CancellationToken cancellation = default)
    {
        List<string> downloadFilePaths = [];
        var versionDownloadHost = $"{config.DownloadHost}{version}/";

        foreach (var file in version.Files)
        {
            var fileDownloadUrl = $"{versionDownloadHost}{file.DownloadUrl}";
            progress?.Report((file.Name, 0));

            var path = await DownloadFileWithProgressAsync(fileDownloadUrl, file.Length, saveFolder, file.Name, progress, cancellation);
            downloadFilePaths.Add(path);
        }

        return downloadFilePaths;
    }

    /// <summary>
    /// 下载文件和进度回调
    /// </summary>
    /// <param name="url"></param>
    /// <param name="totalBytes"></param>
    /// <param name="saveFolder"></param>
    /// <param name="fileName"></param>
    /// <param name="progress"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    private static async Task<string> DownloadFileWithProgressAsync(
        string url, 
        long totalBytes, 
        string saveFolder,
        string fileName, 
        IProgress<(string fileName, double progress)>? progress,
        CancellationToken cancellation = default)
    {
        var savePath = Path.Combine(saveFolder, fileName);
        Directory.CreateDirectory(saveFolder);

        using var response = await url.GetAsync(cancellationToken: cancellation);

        var canReportProgress = totalBytes > 0 && progress != null;

        await using var stream = await response.GetStreamAsync();
        await using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);

        var buffer = new byte[81920]; // 80 KB buffer
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellation)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellation);
            totalRead += bytesRead;

            var progressPercentage = (double)totalRead / totalBytes;
            if (canReportProgress) progress?.Report((fileName, progressPercentage));
        }

        progress?.Report((fileName, 1));
        return savePath;
    }
}