using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Common.Extensions;
using DongKeJi.Common.Inject;
using DongKeJi.Common.UI;
using DongKeJi.Common.ViewModel;
using DongKeJi.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace DongKeJi.ViewModel.User;

/// <summary>
///     用户页面
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class UserDashboardViewModel(
    IContentDialogService contentDialogService,
    ILogger<UserDashboardViewModel> logger,
    ISnackbarService snackbarService,
    IUserRepository userRepository,
    IApplicationContext applicationContext
) : 
    LazyInitializeViewModel, IUserDashboardContext
{
    /// <summary>
    ///     当前登录用户
    /// </summary>
    public UserViewModel LoginUser
    {
        get => applicationContext.LoginUser;
        set => applicationContext.LoginUser = value;
    }

    /// <summary>
    ///     选中用户
    /// </summary>
    [ObservableProperty] private UserViewModel _user = new();

    /// <summary>
    ///     所有用户
    /// </summary>
    [ObservableProperty] private ObservableCollection<UserViewModel> _userList = [];


    protected override async Task OnInitializationAsync(CancellationToken cancellation = default)
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
            var users = await userRepository.GetAllAsync();
            UserList = users.ToObservableCollection();
            User = UserList.FirstOrDefault() ?? UserViewModel.Empty;
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

            LoginUser = UserViewModel.Empty;
            snackbarService.ShowSuccess($"用户已注销: {user.Name}");
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
    private void Login(UserViewModel user)
    {
        try
        {
            LoginUser = user;
            snackbarService.ShowSuccess($"用户已登录: {user.Name}");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "登录用户时发生错误");
        }
    }
}