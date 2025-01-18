using DongKeJi.Inject;
using DongKeJi.Tool.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Tool.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class ToolPage
{
    private readonly IServiceProvider _services;

    public ToolPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();

        LostFocus += async (_, _) =>
        {
            await OnNavigatedFromAsync();
        };
    }


    protected override ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<ToolPageViewModel>();
        DataContext = vm;

        return base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }


    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not ToolPageViewModel vm) return;
        if (args.SelectedItem is ToolItemViewModel tool) vm.SelectedToolItem = tool;
    }
}