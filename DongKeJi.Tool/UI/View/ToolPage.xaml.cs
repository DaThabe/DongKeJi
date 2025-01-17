using DongKeJi.Inject;
using DongKeJi.Tool.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Tool.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class ToolPage
{
    private readonly IServiceProvider _services;

    public ToolPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }


    protected override ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<ToolPageViewModel>();
        DataContext = vm;

        return base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }
}