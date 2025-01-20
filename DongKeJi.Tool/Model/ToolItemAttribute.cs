using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.Model;

/// <summary>
/// 工具元素
/// </summary>
/// <param name="title">标题</param>
/// <param name="icon">图标</param>
[AttributeUsage(AttributeTargets.Class)]
public class ToolItemAttribute(string title, IconElement icon, ServiceLifetime lifetime = ServiceLifetime.Transient) : Attribute, IToolItem
{
    /// <summary>
    /// 标题
    /// </summary>
    public IconElement Icon { get; set; } = icon;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }= title;

    /// <summary>
    /// 页面类型
    /// </summary>
    public Type PageType { get; set; } = typeof(ToolItemAttribute);

    /// <summary>
    /// 视图声明周期
    /// </summary>
    public ServiceLifetime LifeTime { get; set; } = lifetime;


    /// <summary>
    /// 符号图标菜单
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="title"></param>
    /// <param name="lifeTime"></param>
    public ToolItemAttribute(string title, SymbolRegular icon, ServiceLifetime lifeTime = ServiceLifetime.Transient) : 
        this(title, new SymbolIcon(icon), lifeTime)
    {

    }

    /// <summary>
    /// 图片图标菜单
    /// </summary>
    /// <param name="title"></param>
    /// <param name="imageIconSourceUri"></param>
    /// <param name="lifeTime"></param>
    public ToolItemAttribute(string title, string imageIconSourceUri, ServiceLifetime lifeTime = ServiceLifetime.Transient) : 
        this(title, new ImageIcon { Source = new BitmapImage(new Uri(imageIconSourceUri)) }, lifeTime)
    {

    }


    /// <summary>
    /// 验证是否是页面类型
    /// </summary>
    /// <param name="targetType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void ValidatePageType(Type targetType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(targetType))
        {
            throw new InvalidOperationException($"不是有效的页面类型 {nameof(targetType)}.");
        }
    }
}