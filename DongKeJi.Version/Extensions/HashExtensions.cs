using System.IO;
using System.Security.Cryptography;

namespace DongKeJi.Version.Extensions;


public static class HashExtensions
{
    /// <summary>
    /// 计算文件的 MD5 哈希值
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>MD5 哈希值</returns>
    public static async ValueTask<string> CalcFileMd5Async(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"文件未找到: {filePath}");
        }

        using var md5 = MD5.Create();
        await using var stream = File.OpenRead(filePath);
        var hashBytes = await md5.ComputeHashAsync(stream);
        var md5String = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        return md5String;
    }
}