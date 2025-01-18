using System.Windows;
using Wpf.Ui.Appearance;

namespace DongKeJi;


/// <summary>
/// 程序主功能
/// </summary>
public interface IApplication : IApplicationConfig
{
    /// <summary>
    /// 资源字典
    /// </summary>
    ResourceDictionary Resources { get; }

    /// <summary>
    ///     标题
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     版本
    /// </summary>
    Version Version { get; }

    /// <summary>
    ///     主题
    /// </summary>
    ApplicationTheme Theme { get; set; }

    /// <summary>
    /// 关闭程序
    /// </summary>
    void Shutdown();
}