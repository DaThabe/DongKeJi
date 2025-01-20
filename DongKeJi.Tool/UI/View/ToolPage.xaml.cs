using System.Windows;
using DongKeJi.Inject;
using DongKeJi.Tool.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class ToolPage
{
    public ToolPage(ToolPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    
    protected override ValueTask InternalNavigateFromAsync(CancellationToken cancellation = default)
    {
        if (DataContext is ToolPageViewModel vm)
        {
            vm.SelectedToolItem = null;
        }
        return base.InternalNavigateFromAsync(cancellation);
    }


    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not ToolPageViewModel vm) return;
        if (args.SelectedItem is ToolItemViewModel tool) vm.SelectedToolItem = tool;
    }
}