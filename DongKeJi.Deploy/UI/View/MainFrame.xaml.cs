using DongKeJi.Deploy.Service;
using System.Windows;

namespace DongKeJi.Deploy.UI.View;

public partial class MainFrame : Window
{
    private readonly IUpdateService _updateService;

    public MainFrame(IUpdateService updateService)
    {
        _updateService = updateService;
        InitializeComponent();
    }

    public async ValueTask LazyInitAsync()
    {
        await _updateService.UpdateAsync();
    }
}