using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View.User;


[Inject(ServiceLifetime.Transient)]
public partial class UserDashboardView
{
    public UserDashboardView(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }

    protected override async ValueTask<IViewModel> OnLoadViewModelAsync(IServiceProvider services,
        CancellationToken cancellation = default)
    {
        var vm = services.GetRequiredService<UserDashboardObservableViewModel>();
        await vm.InitializeAsync(cancellation);

        return vm;
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not UserDashboardObservableViewModel vm) return;

        if (args.SelectedItem is UserViewModel customer) vm.SelectedUser = customer;
    }
}