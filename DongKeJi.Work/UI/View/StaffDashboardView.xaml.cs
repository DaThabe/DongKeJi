using DongKeJi.Common.Inject;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.ViewModel;
using DongKeJi.ViewModel.User;
using DongKeJi.Work.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

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
        var vm = services.GetRequiredService<StaffDashboardObservableViewModel>();
        await vm.InitializeAsync(cancellation);

        return vm;
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not UserDashboardObservableViewModel vm) return;

        if (args.SelectedItem is UserViewModel customer) vm.SelectedUser = customer;
    }
}