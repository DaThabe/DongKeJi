using DongKeJi.Core.ViewModel;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

namespace DongKeJi.Core.UI.View;


[Inject(ServiceLifetime.Transient)]
public partial class UserPage
{
    private readonly IServiceProvider _services;

    public UserPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<UserPageViewModel>();
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
        if (DataContext is not UserPageViewModel vm) return;

        if (args.SelectedItem is UserViewModel user) vm.SelectedUser = user;
    }

}