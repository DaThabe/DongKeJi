using DongKeJi.Core.ViewModel.Setting;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View.Setting;


[Inject(ServiceLifetime.Transient)]
public partial class SettingDashboardView
{
    private readonly IServiceProvider _services;

    public SettingDashboardView(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<SettingDashboardViewModel>();
        await vm.InitializeAsync(cancellation);

        DataContext = vm;
        await base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }
}