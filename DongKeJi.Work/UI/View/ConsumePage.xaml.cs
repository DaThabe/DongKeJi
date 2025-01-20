using System.Windows;
using DongKeJi.Inject;
using DongKeJi.UI.Control.State;
using DongKeJi.Work.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.UI.View;


[Inject(ServiceLifetime.Transient)]
public partial class ConsumePage
{
    public ConsumePage(ConsumePageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        this.OnLoading(async () => await vm.InitializeAsync());
    }
}