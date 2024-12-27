using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Common.Staff;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

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
        var vm = services.GetRequiredService<StaffPositionDashboardViewModel>();
        await vm.InitializeAsync(cancellation);

        return vm;
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not StaffPositionDashboardViewModel vm) return;
        if (args.SelectedItem is StaffPositionViewModel customer) vm.Position = customer;
    }
}