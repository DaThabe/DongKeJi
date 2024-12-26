using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Common.Inject;
using DongKeJi.Common.UI;
using DongKeJi.Common.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using Wpf.Ui;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Extensions;
using DongKeJi.Common.Extensions;
using DongKeJi.Service;

namespace DongKeJi.ViewModel.User;


/// <summary>
/// 用户页面
/// </summary>
[Inject(ServiceLifetime.Transient)]
public partial class UserDashboardViewModel(IServiceProvider services) : LazyInitializeViewModel
{
    private readonly IUserService _userService = 
        services.GetRequiredService<IUserService>();

    private readonly ISnackbarService _snackbarService = 
        services.GetRequiredService<ISnackbarService>();
    
    private readonly IContentDialogService _contentDialogService = 
        services.GetRequiredService<IContentDialogService>();
    
    private readonly ILogger<UserDashboardViewModel> _logger = 
        services.GetRequiredService<ILogger<UserDashboardViewModel>>();

    /// <summary>
    /// 程序上下文
    /// </summary>
    public IApplicationContext ApplicationContext { get; } = 
        services.GetRequiredService<IApplicationContext>();

    /// <summary>
    /// 所有用户
    /// </summary>
    [ObservableProperty] 
    private UserListViewModel _users = new();


    protected override async Task OnInitializationAsync(CancellationToken cancellation = default)
    {
        await ReloadCommand.ExecuteAsync(null);
    }


    /// <summary>
    /// 刷新
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ReloadAsync()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            Users.Items = users.ToObservableCollection();
            Users.Selected = Users.Items.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _snackbarService.ShowError(ex);
            _logger.LogError(ex, "加载用户时发生错误");
        }
    }


    /// <summary>
    /// 注销
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
                CloseButtonText = "取消",
            };

            //弹窗
            var dialogResult = await _contentDialogService.ShowSimpleDialogAsync(dialog);

            //等待确认
            if (dialogResult != ContentDialogResult.Primary) return;

            ApplicationContext.User.IsLogged = false;
            ApplicationContext.User = UserViewModel.Empty;

            _snackbarService.ShowSuccess($"用户已注销: {user.Name}");
        }
        catch (Exception ex)
        {
            _snackbarService.ShowError(ex);
            _logger.LogError(ex, "注销用户时发生错误");
        }
    }

    /// <summary>
    /// 登录 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [RelayCommand]
    private void Login(UserViewModel user)
    {
        try
        {
            ApplicationContext.User.IsLogged = false;
            ApplicationContext.User = user;
            ApplicationContext.User.IsLogged = true;

            _snackbarService.ShowSuccess($"用户已登录: {user.Name}");
        }
        catch (Exception ex)
        {
            _snackbarService.ShowError(ex);
            _logger.LogError(ex, "登录用户时发生错误");
        }
    }
}