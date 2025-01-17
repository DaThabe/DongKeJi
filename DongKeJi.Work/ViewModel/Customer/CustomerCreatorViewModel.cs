using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.ViewModel;

namespace DongKeJi.Work.ViewModel.Customer;


internal partial class CustomerCreatorViewModel : ObservableViewModel
{
    [ObservableProperty] private CustomerViewModel _customer = new();
}