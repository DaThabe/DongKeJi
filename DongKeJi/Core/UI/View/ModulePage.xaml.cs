using DongKeJi.Core.ViewModel;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

namespace DongKeJi.Core.UI.View;

/// <summary>
/// ModuleDashboardView.xaml 的交互逻辑
/// </summary>
[Inject(ServiceLifetime.Singleton)]
public partial class ModulePage
{
    private readonly IServiceProvider _services;

    public ModulePage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<ModulePageViewModel>();
        DataContext = vm;

        return base.OnNavigatedToAsync(cancellation);
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