using System.Windows;

namespace DongKeJi;

public interface IApplication
{
    /// <summary>
    ///     程序根目录
    /// </summary>
    string BaseDirectory { get; }

    /// <summary>
    ///     数据库文件目录
    /// </summary>
    string DatabaseDirectory { get; }


    /// <summary>
    /// 资源字典
    /// </summary>
    ResourceDictionary Resources { get; }

    /// <summary>
    /// 关闭程序
    /// </summary>
    void Shutdown();
}