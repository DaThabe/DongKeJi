using DongKeJi.Core.ViewModel.Setting;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Core.UI.View.Setting;


[Inject(ServiceLifetime.Transient)]
public partial class SettingDashboardView
{
    public SettingDashboardView(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }

    protected override async ValueTask<IViewModel> OnLoadViewModelAsync(IServiceProvider services,
        CancellationToken cancellation = default)
    {
        var vm = services.GetRequiredService<SettingDashboardObservableViewModel>();
        await vm.InitializeAsync(cancellation);

        return vm;
    }
}