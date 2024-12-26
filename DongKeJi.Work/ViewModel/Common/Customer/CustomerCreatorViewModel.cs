using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.ViewModel.Common.Customer;

[Inject(ServiceLifetime.Transient)]
public partial class CustomerCreatorViewModel : ViewModelBase
{
    [ObservableProperty] private CustomerViewModel _customer = new();
}