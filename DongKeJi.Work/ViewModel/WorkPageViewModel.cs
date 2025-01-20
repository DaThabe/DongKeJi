using DongKeJi.ViewModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.Work.UI.View.Order;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui.Controls;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace DongKeJi.Work.ViewModel;


[Inject(ServiceLifetime.Transient)]
public partial class WorkPageViewModel(
    IServiceProvider services,
    ISnackbarService snackbarService,
    ILogger<WorkPageViewModel> logger,
    IContentDialogService contentDialogService
    ) : LazyInitializeViewModel
{

    /// <summary>
    /// 计息订单信息文本
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ParserOrderInfoTextAsync()
    {
        try
        {
            var creatorContentView = services.GetRequiredService<OrderParserView>();

            var content = new SimpleContentDialogCreateOptions
            {
                Title = "解析订单文本",
                Content = creatorContentView,
                PrimaryButtonText = "创建",
                CloseButtonText = "取消"
            };

            var dialogResult = await contentDialogService.ShowSimpleDialogAsync(content);
            if (dialogResult != ContentDialogResult.Primary) return;
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
            logger.LogError(ex, "订单文本解析失败");
        }
    }
}

//类型：设计
//机构： 壹希医疗美容诊所
//价格：299元
//到账：已到账
//销售：童童
//合作标准:  单张海报
//合作时间：2024.4.4