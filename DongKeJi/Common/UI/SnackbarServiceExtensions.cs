using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace DongKeJi.Common.UI;


public static class SnackbarServiceExtensions
{
    #region --resource--

    /// <summary>
    /// 错误图标符号
    /// </summary>
    public static string ErrorTitle { get; set; } = "ヽ(*。>Д<)o゜";

    /// <summary>
    /// 成功
    /// </summary>
    public static string SuccessTitle { get; set; } = "o(*￣▽￣*)ブ";

    /// <summary>
    /// 正常消息
    /// </summary>
    public static string InfoTitle { get; set; } = "=￣ω￣=";

    /// <summary>
    /// 正常消息
    /// </summary>
    public static string WarningTitle { get; set; } = "(・∀・(・∀・(・∀・*)";


    /// <summary>
    /// 错误图标符号
    /// </summary>
    public static SymbolRegular ErrorSymbol { get; set; } = SymbolRegular.EmojiAngry24;

    /// <summary>
    /// 成功
    /// </summary>
    public static SymbolRegular SuccessSymbol { get; set; } = SymbolRegular.EmojiSparkle24;

    /// <summary>
    /// 正常消息
    /// </summary>
    public static SymbolRegular InfoSymbol { get; set; } = SymbolRegular.EmojiHand24;

    #endregion

   
    public static void Show(this ISnackbarService snackbar, Action<SnackbarConfig> configAction)
    {
        SnackbarConfig config = new();
        configAction(config);
        snackbar.Show(config.Title, config.Message, config.Appearance, config.Icon, config.Timeout);
    }


    public static void ShowWarning(this ISnackbarService snackbar, string message, Action<SnackbarConfig>? configAction = null)
    {
        snackbar.Show(x =>
        {
            x.Title = WarningTitle;
            x.Message = message;
            x.Icon = new SymbolIcon();
            x.Appearance = ControlAppearance.Caution;
            configAction?.Invoke(x);
        });
    }

    public static void ShowSuccess(this ISnackbarService snackbar, string message, Action<SnackbarConfig>? configAction = null)
    {
        snackbar.Show(x =>
        {
            x.Title = SuccessTitle;
            x.Message = message;
            x.Icon = new SymbolIcon(SuccessSymbol);
            x.Appearance = ControlAppearance.Success;
            configAction?.Invoke(x);
        });
    }

    public static void ShowError(this ISnackbarService snackbar, string message, Action<SnackbarConfig>? configAction = null)
    {
        snackbar.Show(x =>
        {
            x.Title = ErrorTitle;
            x.Message = message;
            x.Icon = new SymbolIcon(ErrorSymbol);
            x.Appearance = ControlAppearance.Danger;
            configAction?.Invoke(x);
        });
    }

    public static void ShowError(this ISnackbarService snackbar, Exception exception, Action<SnackbarConfig>? configAction = null)
    {
        snackbar.Show(x =>
        {
            x.Title = ErrorTitle;
            x.Icon = new SymbolIcon(ErrorSymbol);
            x.Appearance = ControlAppearance.Danger;
            x.Message = exception.Message;

            configAction?.Invoke(x);
        });
    }

    public static void ShowInfo(this ISnackbarService snackbar, string message, Action<SnackbarConfig>? configAction = null)
    {
        snackbar.Show(x =>
        {
            x.Title = InfoTitle;
            x.Icon = new SymbolIcon(InfoSymbol);
            x.Appearance = ControlAppearance.Info;
            configAction?.Invoke(x);
        });
    }
}

public class SnackbarConfig
{
    public string Title { get; set; } = "正在弹窗";

    public string Message { get; set; } = string.Empty;

    public ControlAppearance Appearance { get; set; } = ControlAppearance.Transparent;

    public IconElement? Icon { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(2);
}
