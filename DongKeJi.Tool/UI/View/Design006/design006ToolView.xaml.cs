using System.Windows;
using System.Windows.Controls;
using DongKeJi.Tool.Model;
using DongKeJi.Tool.ViewModel.Design006;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Tool.UI.View.Design006;


[ToolItem("享设计", "pack://application:,,,/DongKeJi.Tool;component/UI/Resource/Icon/designer006.png")]
public partial class Design006ToolView 
{
    private readonly IServiceProvider _services;

    public Design006ToolView(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<Design006ViewModel>();
        DataContext = vm;

        return base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }


    private void UIElement_OnDrop(object sender, DragEventArgs e)
    {
        if (DataContext is not Design006ViewModel vm) return;

        if (e.Data.GetDataPresent(DataFormats.Text))
        {
            // 从网页拖拽的文本
            if (e.Data.GetData(DataFormats.Text) is not string url) return;
            vm.DropUrl = new Uri(url);
        }
        else if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            //// 从网页或文件拖拽的文件路径
            //string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //foreach (string file in files)
            //{
            //    MessageBox.Show($"拖拽的文件: {file}");
            //}
        }
    }
}