using DongKeJi.Deploy.Model;
using Flurl.Http;
using System.IO;

namespace DongKeJi.Deploy.Service;

public interface IUpdater
{
    /// <summary>
    /// 获取新版本信息
    /// </summary>
    /// <returns></returns>
    ValueTask<VersionInfo> GetLatestVersionAsync();

    /// <summary>
    /// 下载新版本文件
    /// </summary>
    /// <returns></returns>
    ValueTask DownloadVersionFileAsync(VersionInfo version, string saveFolder, IProgress<(string fileName, double progress)>? progress = null);
}


public class Updater : IUpdater
{
    private const string ServerUrl = "https://gitee.com/dongkeji-cloud/program-hosting/raw/main/";
    private const string VersionFile = "Version.json";
    private const string VersionUrl = $"{ServerUrl}{VersionFile}";

    public async ValueTask<VersionInfo> GetLatestVersionAsync()
    {
        return await VersionUrl.GetJsonAsync<VersionInfo>();
    }

    public async ValueTask DownloadVersionFileAsync(VersionInfo version, string saveFolder, IProgress<(string fileName, double progress)>? progress = null)
    {
        foreach (var file in version.Files)
        {
            var fileDownloadUrl = $"{ServerUrl}{file.DownloadUrl}";
            progress?.Report((file.Name, 0));
            await DownloadFileWithProgressAsync(fileDownloadUrl, file.Length, saveFolder, file.Name, progress);
        }
    }

    private static async Task DownloadFileWithProgressAsync(string url, long totalBytes, string saveFolder, string fileName, IProgress<(string fileName, double progress)>? progress)
    {
        var savePath = Path.Combine(saveFolder, fileName);
        Directory.CreateDirectory(saveFolder);

        using var response = await url.GetAsync();

        var canReportProgress = totalBytes > 0 && progress != null;

        await using var stream = await response.GetStreamAsync();
        await using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);

        var buffer = new byte[81920]; // 80 KB buffer
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;

            var progressPercentage = (double)totalRead / totalBytes;
            if (canReportProgress) progress?.Report((fileName, progressPercentage));
        }

        progress?.Report((fileName, 1));
    }
}