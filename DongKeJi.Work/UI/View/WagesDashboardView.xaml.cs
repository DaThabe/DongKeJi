using DongKeJi.Inject;
using DongKeJi.Work.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.UI.View;


/// <summary>
/// WagesDashboardView.xaml 的交互逻辑
/// </summary>
[Inject(ServiceLifetime.Singleton)]
public partial class WagesDashboardView
{
    private readonly IServiceProvider _services;

    public WagesDashboardView(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<WagesDashboardViewModel>();
        await vm.InitializeAsync(cancellation);
        DataContext = vm;
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }
}
