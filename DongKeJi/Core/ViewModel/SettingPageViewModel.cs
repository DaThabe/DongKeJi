using CommunityToolkit.Mvvm.Input;
using DongKeJi.Core.Service;
using DongKeJi.Core.UI.View.Update;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace DongKeJi.Core.ViewModel;


/// <summary>
///     设置
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class SettingPageViewModel(
    ILogger<SettingPageViewModel> logger,
    IApplication application,
    IContentDialogService contentDialogService,
    ISnackbarService snackbarService,
    IUpdateService updateService
    ) : LazyInitializeViewModel
{
    /// <summary>
    ///     程序
    /// </summary>
    public IApplication Application => application;


    /// <summary>
    /// 改变主题
    /// </summary>
    /// <param name="name"></param>
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

    /// <summary>
    /// 检查更新
    /// </summary>
    [RelayCommand]
    private async Task CheckUpdateAsync()
    {
        try
        {
            //检查版本
            var versionList = await updateService.GetVersionListAsync();
            var latestVersionItem = versionList.Versions.Find(x => x.Version == versionList.LatestVersion);
            ArgumentNullException.ThrowIfNull(latestVersionItem);

            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "发现新版本",
                PrimaryButtonText = "更新",
                CloseButtonText = "取消",
                Content = new VersionView { DataContext = latestVersionItem }
            };

            var result = await contentDialogService.ShowSimpleDialogAsync(dialog);
            if (result != ContentDialogResult.Primary) return;

            //更新程序
            await updateService.UpdateAsync();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "检查版本时发生错误");
        }
    }
}