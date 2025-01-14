using System.Windows.Controls;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class ConsumeDashboardView
{
    public ConsumeDashboardView()
    {
        InitializeComponent();
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not UserDashboardViewModel vm) return;

        if (args.SelectedItem is UserViewModel customer) vm.SelectedUser = customer;
    }
}