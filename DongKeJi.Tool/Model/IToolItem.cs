using Wpf.Ui.Controls;

namespace DongKeJi.Tool.Model;

/// <summary>
/// 工具元素
/// </summary>
public interface IToolItem
{
    /// <summary>
    /// 图标
    /// </summary>
    IconElement Icon { get; }

    /// <summary>
    /// 标题
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 页面类型
    /// </summary>
    Type PageType { get; }
}