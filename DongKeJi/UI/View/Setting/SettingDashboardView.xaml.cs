using DongKeJi.Common.Inject;
using DongKeJi.Common.ViewModel;
using DongKeJi.ViewModel.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.UI.View.Setting;


[Inject(ServiceLifetime.Transient)]
public partial class SettingDashboardView
{
    public SettingDashboardView(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }

    protected override async ValueTask<IViewModel> OnLoadViewModelAsync(IServiceProvider services, CancellationToken cancellation = default)
    {
        var vm = services.GetRequiredService<SettingDashboardViewModel>();
        await vm.InitializeAsync(cancellation);
        
        return vm;
    }
}
