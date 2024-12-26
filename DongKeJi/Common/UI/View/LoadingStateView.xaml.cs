using System.Windows.Controls;

namespace DongKeJi.Common.UI.View;

/// <summary>
/// ViewStateControl.xaml 的交互逻辑
/// </summary>
public partial class LoadingStateView : UserControl
{
    public LoadingStateView()
    {
        InitializeComponent();
        this.Message.Text = "";
        this.Message.Visibility = System.Windows.Visibility.Collapsed;
    }

    public LoadingStateView(string message)
    {
        InitializeComponent();
        this.Message.Text = message;
        this.Message.Visibility = System.Windows.Visibility.Visible;
    }
}
