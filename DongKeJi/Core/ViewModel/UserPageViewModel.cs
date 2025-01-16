using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Core.Service;
using DongKeJi.Core.ViewModel.User;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace DongKeJi.Core.ViewModel;

/// <summary>
///     用户页面
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class UserPageViewModel(
    IContentDialogService contentDialogService,
    ILogger<UserPageViewModel> logger,
    ISnackbarService snackbarService,
    IUserService userService,
    ICoreModule coreModule,
    ICoreDatabase database) : LazyInitializeViewModel
{
    /// <summary>
    ///     当前登录用户
    /// </summary>
    public UserViewModel CurrentUser => 
        coreModule.CurrentUser ?? throw new ArgumentNullException(nameof(CurrentUser), "当前用户为空");

    /// <summary>
    ///     选中用户
    /// </summary>
    [ObservableProperty] private UserViewModel? _selectedUser;

    /// <summary>
    ///     所有用户
    /// </summary>
    [ObservableProperty] private ObservableCollection<UserViewModel> _userCollection = [];


    protected override async ValueTask OnInitializationAsync(CancellationToken cancellation = default)
    {
        await ReloadCommand.ExecuteAsync(null);
    }


    /// <summary>
    ///     刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadAsync()
    {
        try
        {
            var users = await userService.GetAllAsync();


            UserCollection = users.ToObservableCollection();
            EnumerableExtensions.ForEach<UserViewModel>(UserCollection, x=> database.AutoUpdate(x));
            SelectedUser = Enumerable.FirstOrDefault<UserViewModel>(UserCollection);
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "加载用户时发生错误");
        }
    }


    /// <summary>
    ///     注销
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task LogoutAsync(UserViewModel user)
    {
        try
        {
            SimpleContentDialogCreateOptions dialog = new()
            {
                Title = "是否注销用户",
                Content = $"用户名称: {user.Name}",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消"
            };

            //弹窗
            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;
            await userService.LogoutAsync();
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "注销用户时发生错误");
        }
    }

    /// <summary>
    ///     登录
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [RelayCommand]
    private async Task LoginAsync(UserViewModel user)
    {
        try
        {
            await userService.LoginAsync(user);
            snackbarService.ShowSuccess($"用户已登录: {user.Name}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "登录用户时发生错误");
        }
    }
}