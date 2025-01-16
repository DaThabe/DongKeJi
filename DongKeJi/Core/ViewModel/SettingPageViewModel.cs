using CommunityToolkit.Mvvm.Input;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace DongKeJi.Core.ViewModel;


/// <summary>
///     设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class SettingPageViewModel(IApplication application) : LazyInitializeViewModel
{
    /// <summary>
    ///     程序
    /// </summary>
    public IApplication Application => application;




    [RelayCommand]
    private void ChangeTheme(string name)
    {
        switch (name)
        {
            case nameof(ApplicationTheme.Light):
                application.Theme = ApplicationTheme.Light;
                return;

            case nameof(ApplicationTheme.Dark):
                application.Theme = ApplicationTheme.Dark;
                return;

            case nameof(ApplicationTheme.HighContrast):
                application.Theme = ApplicationTheme.HighContrast;
                return;
        }
    }
}