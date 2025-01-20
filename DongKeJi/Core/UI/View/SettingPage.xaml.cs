using DongKeJi.Core.ViewModel;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Core.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class SettingPage
{
    public SettingPage(SettingPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}