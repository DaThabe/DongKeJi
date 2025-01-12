using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Customer;


[Inject(ServiceLifetime.Transient)]
public partial class CustomerCreatorObservableViewModel : ObservableViewModel
{
    [ObservableProperty] private CustomerViewModel _customer = new();
}