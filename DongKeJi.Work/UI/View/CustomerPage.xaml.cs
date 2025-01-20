using System.Windows;
using DongKeJi.Inject;
using DongKeJi.UI.Control.State;
using DongKeJi.Work.ViewModel;
using DongKeJi.Work.ViewModel.Customer;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;

[Inject(ServiceLifetime.Transient)]
public partial class CustomerPage
{
    public CustomerPage(CustomerPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        this.OnLoading(async () => await vm.InitializeAsync());
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not CustomerPageViewModel vm) return;
        if (args.SelectedItem is CustomerViewModel customer) vm.SelectedCustomer = customer;
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