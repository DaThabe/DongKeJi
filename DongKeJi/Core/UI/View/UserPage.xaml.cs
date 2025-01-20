using DongKeJi.Core.ViewModel;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.UI.Control.State;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class UserPage
{
    public UserPage(UserPageViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        this.OnLoading(async() => await vm.InitializeAsync());
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not UserPageViewModel vm) return;

        if (args.SelectedItem is UserViewModel user) vm.SelectedUser = user;
    }
}