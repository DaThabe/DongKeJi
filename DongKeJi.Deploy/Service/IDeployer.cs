using System.IO;
using DongKeJi.Deploy.Extensions;
using DongKeJi.Deploy.Model;

namespace DongKeJi.Deploy.Service;

public interface IDeployer
{
    /// <summary>
    /// 验证文件
    /// </summary>
    /// <param name="info">版本信息</param>
    /// <param name="files">版本文件</param>
    /// <returns></returns>
    ValueTask ValidateByFilePathsAsync(VersionInfo info, IEnumerable<string> files);
}

public static class DeployerExtensions
{
    /// <summary>
    /// 从目录验证
    /// </summary>
    /// <param name="deployer"></param>
    /// <param name="info">版本信息</param>
    /// <param name="folder">所有更新文件所在目录</param>
    /// <returns></returns>
    public static  ValueTask ValidateByFolderAsync(
        this IDeployer deployer,
        VersionInfo info, 
        string folder)
    {
        var files = Directory.EnumerateFiles(folder);
        return deployer.ValidateByFilePathsAsync(info, files);
    }
}


public class Deployer : IDeployer
{
    public async ValueTask ValidateByFilePathsAsync(VersionInfo info, IEnumerable<string> files)
    {
        Dictionary<string, string> fileNamePath = new(files.Select(x =>
        {
            var fileName = Path.GetFileName(x);
            return new KeyValuePair<string, string>(fileName, x);
        }));


        foreach (var i in info.Files)
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