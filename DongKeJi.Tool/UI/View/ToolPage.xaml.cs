using DongKeJi.Inject;
using DongKeJi.Tool.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class ToolPage : INavigationAware
{
    public ToolPage(ToolPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public void OnNavigatedTo()
    {
        
    }

    public void OnNavigatedFrom()
    {
        if (DataContext is ToolPageViewModel vm)
        {
            vm.SelectedToolItem = null;
        }
    }


    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not ToolPageViewModel vm) return;
        if (args.SelectedItem is ToolItemViewModel tool) vm.SelectedToolItem = tool;
    }
}