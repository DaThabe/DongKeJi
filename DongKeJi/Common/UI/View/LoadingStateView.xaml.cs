using System.Windows;
using System.Windows.Controls;

namespace DongKeJi.Common.UI.View;

/// <summary>
///     ViewStateControl.xaml 的交互逻辑
/// </summary>
public partial class LoadingStateView : UserControl
{
    public LoadingStateView()
    {
        InitializeComponent();
        Message.Text = "";
        Message.Visibility = Visibility.Collapsed;
    }

    public LoadingStateView(string message)
    {
        InitializeComponent();
        Message.Text = message;
        Message.Visibility = Visibility.Visible;
    }
}