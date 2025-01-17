using DongKeJi.Core.ViewModel;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;


[Inject(ServiceLifetime.Transient)]
public partial class StaffPage
{
    private readonly IServiceProvider _services;

    public StaffPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<StaffPageViewModel>();
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
        if (DataContext is not StaffPageViewModel vm) return;
        if (args.SelectedItem is StaffViewModel staff) vm.SelectedStaff = staff;
    }
}