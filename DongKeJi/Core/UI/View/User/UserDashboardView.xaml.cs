using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.UI.View;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View.User;


[Inject(ServiceLifetime.Transient)]
public partial class UserDashboardView
{
    private readonly IServiceProvider _services;

    public UserDashboardView(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<UserDashboardViewModel>();
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
        if (DataContext is not UserDashboardViewModel vm) return;

        if (args.SelectedItem is UserViewModel user) vm.SelectedUser = user;
    }

}