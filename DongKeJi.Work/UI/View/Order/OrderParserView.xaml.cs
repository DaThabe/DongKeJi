using DongKeJi.Inject;
using DongKeJi.Work.ViewModel.Order;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.Work.UI.View.Order;


[Inject(ServiceLifetime.Transient)]
public partial class OrderParserView
{
    public OrderParserView(OrderParserViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}