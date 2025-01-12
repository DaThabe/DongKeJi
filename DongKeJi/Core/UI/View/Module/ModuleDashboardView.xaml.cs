﻿using DongKeJi.Core.ViewModel.Module;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View.Module;

/// <summary>
/// ModuleDashboardView.xaml 的交互逻辑
/// </summary>
[Inject(ServiceLifetime.Singleton)]
public partial class ModuleDashboardView
{
    private readonly IServiceProvider _services;

    public ModuleDashboardView(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<ModuleDashboardViewModel>();
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
        if (DataContext is not UserDashboardViewModel vm) return;

        if (args.SelectedItem is UserViewModel user) vm.SelectedUser = user;
    }
}