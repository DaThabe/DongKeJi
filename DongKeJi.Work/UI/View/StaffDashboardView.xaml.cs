using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel.User;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using StaffDashboardViewModel = DongKeJi.Work.ViewModel.StaffDashboardViewModel;

namespace DongKeJi.Work.UI.View;

[Inject(ServiceLifetime.Transient)]
public partial class StaffDashboardView
{
    public StaffDashboardView(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }

    protected override async ValueTask<IViewModel> OnLoadViewModelAsync(IServiceProvider services,
        CancellationToken cancellation = default)
    {
        var vm = services.GetRequiredService<StaffDashboardViewModel>();
        await vm.InitializeAsync(cancellation);

        return vm;
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not UserDashboardViewModel vm) return;

        if (args.SelectedItem is UserViewModel customer) vm.Users.Selected = customer;
    }
}