using System.IO;
using DongKeJi.Deploy.Extensions;
using DongKeJi.Deploy.Model;

namespace DongKeJi.Deploy.Service;

public interface IDeployService
{
    /// <summary>
    /// 验证文件
    /// </summary>
    /// <param name="item">版本信息</param>
    /// <param name="files">版本文件</param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    ValueTask ValidateByFilePathsAsync(
        VersionItem item, 
        IEnumerable<string> files,
        CancellationToken cancellation = default);
}

public static class DeployExtensions
{
    /// <summary>
    /// 从目录验证
    /// </summary>
    /// <param name="deployService"></param>
    /// <param name="item">版本信息</param>
    /// <param name="folder">所有更新文件所在目录</param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static ValueTask ValidateByFolderAsync(
        this IDeployService deployService,
        VersionItem item, 
        string folder,
        CancellationToken cancellation = default)
    {
        var files = Directory.EnumerateFiles(folder);
        return deployService.ValidateByFilePathsAsync(item, files, cancellation);
    }
}


public class DeployService : IDeployService
{
    public async ValueTask ValidateByFilePathsAsync(
        VersionItem item, 
        IEnumerable<string> files,
        CancellationToken cancellation = default)
    {
        Dictionary<string, string> fileNamePath = new(files.Select(x =>
        {
            var fileName = Path.GetFileName(x);
            return new KeyValuePair<string, string>(fileName, x);
        }));


        foreach (var i in item.Files)
        {
            if (!fileNamePath.TryGetValue(i.Name, out var path))
            {
                throw new FileNotFoundException($"缺少文件: {i.Name}");
            }

            var md5 = await HashExtensions.CalcFileMd5Async(path);
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
}