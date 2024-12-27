using System.Windows;
using System.Windows.Controls;

namespace DongKeJi.Common.UI.View;


/// <summary>
/// AsyncView.xaml 的交互逻辑
/// </summary>
public partial class AsyncView : UserControl
{
    public static readonly DependencyProperty MyPropertyProperty =
        DependencyProperty.Register(
            nameof(Background),
            typeof(FrameworkElement),
            typeof(AsyncView),
            new PropertyMetadata(null));


    public FrameworkElement MaskContent
    {
        get => (FrameworkElement)GetValue(MyPropertyProperty);
        set => SetValue(MyPropertyProperty, value);
    }
    public AsyncView()
    {
        InitializeComponent();
    }
}