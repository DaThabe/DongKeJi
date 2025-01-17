using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DongKeJi.Inject;
using DongKeJi.UI;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace DongKeJi.Work.ViewModel.Consume;


[Inject(ServiceLifetime.Transient)]
public partial class ConsumeExporterViewModel(
    ILogger<ConsumeExporterViewModel> logger,
    ISnackbarService snackbarService,
    IWorkModule workModule
) : ObservableViewModel
{
    #region --上下文--
   
    [ObservableProperty] private DateTime _date = DateTime.Now;

    [ObservableProperty] private string? _savePath;

    [ObservableProperty] private string _exportContent = "";

    #endregion

    #region --命令--

    /// <summary>
    /// 选择导出目录
    /// </summary>
    [RelayCommand]
    private void SelectedSaveFolder()
    {
        try
        {
            OpenFolderDialog openFolderDialog = new()
            {
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFolderDialog.ShowDialog() != true) return;
            if (openFolderDialog.FolderNames.Length == 0) return;

            var fileName = string.Join("-", workModule.CurrentStaff?.Name, Date.ToString("yyyy-MM"), "服务明细.txt");
            SavePath = Path.Combine(openFolderDialog.FolderNames.First(), fileName);
        }
        catch (Exception ex)
        {
            SavePath = "";
            logger.LogError(ex, "选择储存目录时发生错误");
            snackbarService.ShowError("路径无效, 请重新选择");
        }
    }

    /// <summary>
    /// 保存到文件
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveToFileAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ExportContent))
            {
                snackbarService.Show("保存失败", "没有信息可以导出");
                return;
            }

            if (string.IsNullOrWhiteSpace(SavePath))
            {
                snackbarService.Show("保存失败", "请选择储存目录");
                return;
            }

            var folder = Path.GetDirectoryName(SavePath);
            if (Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            await File.WriteAllTextAsync(SavePath, ExportContent);

            snackbarService.ShowSuccess($"保存成功, 已储存至: {SavePath}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "保存到文件时发生错误");
            snackbarService.ShowError(ex);
        }
    }

    /// <summary>
    /// 打开已经保存的文件
    /// </summary>
    [RelayCommand]
    private void OpenSavedFile()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                Process.Start("explorer.exe", $"/select,\"{SavePath}\"");
                return;
            }

            snackbarService.Show("文件不存在", "无法打开");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "打开已经保存明细文件时发生错误");
            snackbarService.Show("明细文件打开失败", "请检查文件是否存在");
        }
    }

    #endregion
}