using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace DongKeJi.ViewModel;

public partial class ApplicationViewModel : ObservableObject
{
    private readonly IThemeService _themeService;

    /// <summary>
    ///     主题
    /// </summary>
    [ObservableProperty] private ApplicationTheme _theme;


    /// <summary>
    ///     标题
    /// </summary>
    [ObservableProperty] private string _title;

    /// <summary>
    ///     版本
    /// </summary>
    [ObservableProperty] private Version _version;


    /// <inheritdoc />
    public ApplicationViewModel(IThemeService themeService)
    {
        _themeService = themeService;
        var assemblyName = Assembly.GetExecutingAssembly().GetName();

        Title = assemblyName.Name ?? nameof(ApplicationViewModel);
        Version = assemblyName.Version ?? new Version();
        Theme = ApplicationTheme.Light;
    }


    [RelayCommand]
    private void ChangeTheme(string name)
    {
        switch (name)
        {
            case nameof(ApplicationTheme.Light):
                Theme = ApplicationTheme.Light;
                return;

            case nameof(ApplicationTheme.Dark):
                Theme = ApplicationTheme.Dark;
                return;

            case nameof(ApplicationTheme.HighContrast):
                Theme = ApplicationTheme.HighContrast;
                return;
        }
    }


    partial void OnThemeChanged(ApplicationTheme value)
    {
        _themeService.SetTheme(value);
    }
}