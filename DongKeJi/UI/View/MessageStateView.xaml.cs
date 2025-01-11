using System.Windows.Controls;

namespace DongKeJi.UI.View;

/// <summary>
///     ViewMessageStateControl.xaml 的交互逻辑
/// </summary>
public partial class MessageStateView : UserControl
{
    public MessageStateView(string title, string message)
    {
        InitializeComponent();
        Title.Text = title;
        Message.Text = message;
    }
}