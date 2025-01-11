using DongKeJi.Common.Inject;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using StaffPositionViewModel = DongKeJi.Work.ViewModel.Staff.StaffPositionViewModel;

namespace DongKeJi.Work.UI.View;

[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionDashboardView
{
    public StaffPositionDashboardView(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }

    protected override async ValueTask<IViewModel> OnLoadViewModelAsync(IServiceProvider services,
        CancellationToken cancellation = default)
    {
        var vm = services.GetRequiredService<StaffPositionDashboardObservableViewModel>();
        await vm.InitializeAsync(cancellation);

        return vm;
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not StaffPositionDashboardObservableViewModel vm) return;
        if (args.SelectedItem is StaffPositionViewModel customer) vm.Position = customer;
    }
}