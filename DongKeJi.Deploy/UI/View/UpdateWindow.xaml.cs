using DongKeJi.Deploy.Service;
using System.Windows;

namespace DongKeJi.Deploy.UI.View;

/// <summary>
/// 更新窗口
/// </summary>
public partial class UpdateWindow
{
    private readonly IUpdateService _updateService;

    public UpdateWindow(IUpdateService updateService)
    {
        _updateService = updateService;
        InitializeComponent();
    }

    public async ValueTask LazyInitAsync() => await _updateService.UpdateAsync();
}