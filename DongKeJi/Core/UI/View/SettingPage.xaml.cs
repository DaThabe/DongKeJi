using DongKeJi.Core.ViewModel;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.UI.View;


[Inject(ServiceLifetime.Transient)]
public partial class SettingPage
{
    private readonly IServiceProvider _services;

    public SettingPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<SettingPageViewModel>();
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