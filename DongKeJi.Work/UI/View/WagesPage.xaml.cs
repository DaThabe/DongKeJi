using System.Windows;
using DongKeJi.Inject;
using DongKeJi.UI.Control.State;
using DongKeJi.Work.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.UI.View;


/// <summary>
/// WagesDashboardView.xaml 的交互逻辑
/// </summary>
[Inject(ServiceLifetime.Singleton)]
public partial class WagesPage
{
    public WagesPage(WagesPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        this.OnLoading(async () => await vm.InitializeAsync());
    }
}
