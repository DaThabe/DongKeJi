using System.Windows;
using DongKeJi.Inject;
using DongKeJi.UI.Control.State;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Staff;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;

[Inject(ServiceLifetime.Transient)]
public partial class StaffPositionPage
{
    public StaffPositionPage(StaffPositionPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        this.OnLoading(async () => await vm.InitializeAsync());
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not StaffPositionPageViewModel vm) return;
        if (args.SelectedItem is StaffPositionViewModel position) vm.SelectedPosition = position;
    }
}