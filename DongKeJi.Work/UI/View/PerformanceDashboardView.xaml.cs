using System.Windows;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.Work.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;

[Inject(ServiceLifetime.Transient)]
public partial class PerformanceDashboardView
{
    private readonly IServiceProvider _services;

    public PerformanceDashboardView(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<PerformanceDashboardViewModel>();
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

        if (args.SelectedItem is UserViewModel customer) vm.SelectedUser = customer;
    }

    private void AddOrderButtonClick(object sender, RoutedEventArgs e)
    {
        OrderLayout.SelectedIndex = 0;
    }

    private void AddConsumeButtonClick(object sender, RoutedEventArgs e)
    {
        OrderLayout.SelectedIndex = 1;
    }
}