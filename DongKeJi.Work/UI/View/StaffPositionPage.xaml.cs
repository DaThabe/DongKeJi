using DongKeJi.Inject;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;

[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionPage
{
    private readonly IServiceProvider _services;

    public StaffPositionPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<StaffPositionPageViewModel>();
        await vm.InitializeAsync(cancellation);

        DataContext = vm;

        await base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not StaffPositionPageViewModel vm) return;
        if (args.SelectedItem is StaffPositionViewModel position) vm.SelectedPosition = position;
    }
}