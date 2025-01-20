using DongKeJi.Core.ViewModel;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View;

/// <summary>
/// ModuleDashboardView.xaml 的交互逻辑
/// </summary>
[Inject(ServiceLifetime.Singleton)]
public partial class ModulePage
{
    public ModulePage(ModulePageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    //protected override ValueTask InternalNavigateToAsync(CancellationToken cancellation = default)
    //{
    //    var vm = _services.GetRequiredService<ModulePageViewModel>();
    //    DataContext = vm;

    //    return base.InternalNavigateToAsync(cancellation);
    //}

    //protected override ValueTask InternalNavigateFromAsync(CancellationToken cancellation = default)
    //{
    //    DataContext = null;
    //    return base.InternalNavigateFromAsync(cancellation);
    //}

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not UserPageViewModel vm) return;

        if (args.SelectedItem is UserViewModel user) vm.SelectedUser = user;
    }

}