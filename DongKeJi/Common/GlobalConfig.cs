using System.IO;

namespace DongKeJi.Common;

public static class GlobalConfig
{
    /// <summary>
    /// 程序根目录
    /// </summary>
    public static string ApplicationBaseDirectory { get; } = AppContext.BaseDirectory;

    /// <summary>
    /// 数据库文件目录
    /// </summary>
    public static string DatabaseDirectory { get; } = Path.Combine(ApplicationBaseDirectory, "Database");
}
