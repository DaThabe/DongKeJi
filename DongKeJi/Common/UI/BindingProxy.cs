using System.Windows;

namespace DongKeJi.Common.UI;

/// <summary>
///     绑定代理
/// </summary>
public class BindingProxy : Freezable
{
    // 注册 Data 依赖属性
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    // 使用依赖属性来存储绑定的数据上下文
    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }
}