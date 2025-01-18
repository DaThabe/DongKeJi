using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Extensions;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using Flurl.Http;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace DongKeJi.Tool.ViewModel.Design006;


[Inject(ServiceLifetime.Transient)]
public partial class Design006ViewModel(
    IApplication application,
    ISnackbarService snackbarService
    ) : ObservableViewModel
{
    #region --数据上下文--

    /// <summary>
    /// 拖入的Url
    /// </summary>
    [ObservableProperty] 
    private Uri? _dropUrl;


    partial void OnDropUrlChanged(Uri? value)
    {
        try
        {
            if (value is null) return;
            
            var match = Regex.Match(value.AbsoluteUri);
            if (!match.Success) throw new Exception("并非支持的享设计链接");

            DropUrl = new Uri(match.Value);
        }
        catch (Exception e)
        {
            DropUrl = null;
            snackbarService.ShowError(e);
        }
    }

    #endregion

    #region --命令--

    /// <summary>
    /// 拷贝图片
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task CopyAsync()
    {
        try
        {
            var uri = await DownloadPicture();

            BitmapImage image = new(new Uri(uri));
            Clipboard.SetImage(image);

            snackbarService.ShowSuccess("已复制到剪切板");
        }
        catch(Exception ex)
        {
            snackbarService.ShowError(ex);
        }
    }

    /// <summary>
    /// 打开图片
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    [RelayCommand]
    private async Task OpenAsync()
    {
        try
        {
            var uri = await DownloadPicture();
            if (!File.Exists(uri))
            {
                throw new FileNotFoundException("文件不存在", uri);
            }

            await Task.Delay(100);

            Process.Start("explorer.exe", $"/select,\"{uri}\"");
        }
        catch (Exception ex)
        {
            snackbarService.ShowError(ex);
        }
    }

    /// <summary>
    /// 清除当前图片
    /// </summary>
    [RelayCommand]
    private void Clear()
    {
        DropUrl = null;
    }

    #endregion


    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="useLocalFile">是否使用本地文件, 如果下载的存在则跳过</param>
    /// <returns></returns>
    private async ValueTask<string> DownloadPicture(bool useLocalFile = true)
    {
        ArgumentNullException.ThrowIfNull(DropUrl);

        var fileId = DropUrl.AbsoluteUri.ToMd5();
        var fileType = Path.GetExtension(DropUrl.AbsoluteUri);

        var fileName = $"{fileId}{fileType}";
        var filePath = Path.Combine(application.DirectoryCache, fileName);

        if (!Directory.Exists(application.DirectoryCache))
        {
            Directory.CreateDirectory(application.DirectoryCache);
        }

        if (useLocalFile && File.Exists(filePath))
        {
            return filePath;
        }

        return await DropUrl.DownloadFileAsync(application.DirectoryCache, fileName);
    }



    //https://imgs.design006.com/202501/Design006_mdzWQB7R4h.jpg!prev_w_750_h_auto
    private static readonly Regex Regex = new(@"^https://imgs\.design006\.com/.*?\.(jpg|png|gif)");
}