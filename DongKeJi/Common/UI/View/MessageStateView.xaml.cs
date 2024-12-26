using System.Windows.Controls;

namespace DongKeJi.Common.UI.View;

/// <summary>
/// ViewMessageStateControl.xaml 的交互逻辑
/// </summary>
public partial class MessageStateView : UserControl
{
    public MessageStateView(string title, string message)
    {
        InitializeComponent();
        this.Title.Text = title;
        this.Message.Text = message;
    }
}
