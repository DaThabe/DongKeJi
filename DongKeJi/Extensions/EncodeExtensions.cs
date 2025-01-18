using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DongKeJi.Extensions;

/// <summary>
/// Base64编码内容
/// </summary>
public interface IBase64EncodeContent : IEncodeContent<string>;

/// <summary>
/// Json编码内容
/// </summary>
public interface IJsonEncodeContent : IEncodeContent<string>;

/// <summary>
/// Md5编码内容
/// </summary>
public interface IMd5EncodeContent : IEncodeContent<string>;


public static class EncodeExtensions
{
    /// <summary>
    /// 转为Base64
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IBase64EncodeContent ToBase64<T>(this T value) where T : notnull
    {
        var jsonString = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        var base64String =  Convert.ToBase64String(bytes);

        return new Base64EncodeContent { Content = base64String };
    }

    /// <summary>
    /// 转为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="base64Encode"></param>
    /// <returns></returns>
    public static T? ToObject<T>(this IBase64EncodeContent base64Encode) where T : notnull
    {
        var bytes = Convert.FromBase64String(base64Encode.Content);
        var jsonString = Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<T>(jsonString);
    }



    /// <summary>
    /// 转为Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IJsonEncodeContent ToJson<T>(this T value) where T : notnull
    {
        var jsonString = JsonSerializer.Serialize(value);
        return new JsonEncodeContent { Content = jsonString };
    }

    /// <summary>
    /// 转为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? ToObject<T>(this IJsonEncodeContent json) where T : notnull
    {
        return JsonSerializer.Deserialize<T>(json.Content);
    }


    /// <summary>
    /// 转为Md5
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IMd5EncodeContent ToMd5<T>(this T value) where T : notnull
    {
        // 1. 将对象序列化为 JSON 字符串
        var jsonString = JsonSerializer.Serialize(value);

        // 2. 将 JSON 字符串转换为字节数组
        var inputBytes = Encoding.UTF8.GetBytes(jsonString);

        // 3. 计算 MD5 哈希值
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(inputBytes);

        // 4. 转换为十六进制字符串表示
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2")); // 转为两位小写十六进制格式
        }

        return new Md5EncodeContent { Content = sb.ToString() };
    }

    /// <summary>
    /// 计算文件的 MD5 哈希值
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="cancellation"></param>
    /// <returns>MD5 哈希值</returns>
    public static async ValueTask<string> CalcFileMd5Async(
        string filePath, 
        CancellationToken cancellation = default)
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



/// <summary>
/// 编码内容
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEncodeContent<out T>
{
    T Content { get; }
}

file class Base64EncodeContent : IBase64EncodeContent
{
    public required string Content { get; init; }

    public override string ToString() => Content;
}

file class JsonEncodeContent : IJsonEncodeContent
{
    public required string Content { get; init; }

    public override string ToString() => Content;
}

file class Md5EncodeContent : IMd5EncodeContent
{
    public required string Content { get; init; }


    public override string ToString() => Content;
}