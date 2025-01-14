using System.Windows.Controls;
using DongKeJi.Inject;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class WorkDashboardView : UserControl
{
    public WorkDashboardView()
    {
        InitializeComponent();
    }
}