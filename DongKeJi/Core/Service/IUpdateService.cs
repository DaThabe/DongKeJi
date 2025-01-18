using System.Data;
using Flurl.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using DongKeJi.Core.Model.Update;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.Service;

/// <summary>
/// 更新服务
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// 更新最新版本
    /// </summary>
    /// <returns></returns>
    ValueTask UpdateAsync(
        CancellationToken cancellation = default);

    /// <summary>
    /// 获取版本列表
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask<VersionList> GetVersionListAsync(
        CancellationToken cancellation = default);
}


[Inject(ServiceLifetime.Singleton, typeof(IUpdateService))]
internal class UpdateService(IApplication application) : IUpdateService
{
    public async ValueTask UpdateAsync(
        CancellationToken cancellation = default)
    {
        //所有版本
        var versionList = await GetVersionListAsync(cancellation);

        //本地版本
        var currentVersion = application.Version;
        //最新版本
        var latestVersion = versionList.LatestVersion;

        //已经是最新版本
        if (currentVersion >= latestVersion)
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

        //验证下载的文件
        await ValidateByFilePathsAsync(latestVersionItem, files, cancellation);
    }

    public async ValueTask<VersionList> GetVersionListAsync(
        CancellationToken cancellation = default)
    {
        return await application.UpdateVersionListUrl.GetJsonAsync<VersionList>(cancellationToken: cancellation);
    }


    private static async ValueTask ValidateByFilePathsAsync(
        VersionItem version,
        IEnumerable<string> files,
        CancellationToken cancellation = default)
    {
        Dictionary<string, string> fileNamePath = new(files.Select(x =>
        {
            var fileName = Path.GetFileName(x);
            return new KeyValuePair<string, string>(fileName, x);
        }));


        foreach (var i in version.Files)
        {
            if (!fileNamePath.TryGetValue(i.Name, out var path))
            {
                throw new FileNotFoundException($"缺少文件: {i.Name}");
            }

            var md5 = await EncodeExtensions.CalcFileMd5Async(path, cancellation);
            if (!i.Md5.Equals(md5, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new FileLoadException($"非预期文件: {i.Name}\n{path}");
            }

            var length = new FileInfo(path).Length;
            if (i.Length != length)
            {
                throw new FileLoadException($"文件非预期长度: {length}");
            }
        }
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
        var versionDownloadHost = $"{application.UpdateDownloadHost}{version.Version}/";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("referer", "https://gitee.com/dongkeji-cloud/release/blob/master/Version/");
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

        foreach (var file in version.Files)
        {
            var fileDownloadUrl = $"{versionDownloadHost}{file.DownloadUrl}";
            progress?.Report((file.Name, 0));

            var bytes = await client.GetByteArrayAsync(fileDownloadUrl, cancellation);

            var filePath = Path.Combine(saveFolder, file.Name);
            await File.WriteAllBytesAsync(filePath, bytes, cancellation);

           // var path = await DownloadFileWithProgressAsync(fileDownloadUrl, file.Length, saveFolder, file.Name, progress, cancellation);
            downloadFilePaths.Add(filePath);
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