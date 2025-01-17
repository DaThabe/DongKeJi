using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Inject;
using DongKeJi.Work.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Work.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class ConsumePage
{
    private readonly IServiceProvider _services;

    public ConsumePage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override async ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<ConsumePageViewModel>();
        await vm.InitializeAsync(cancellation);

        DataContext = vm;
    }
    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        DataContext = null;
        return ValueTask.CompletedTask;
    }
}