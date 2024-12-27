﻿using System.Windows;
using System.Windows.Controls;

namespace DongKeJi.Common.UI.View;

public static class ContentControlExtensions
{
    /// <summary>
    ///     异步加载DataContext  (传入异步过程
    /// </summary>
    /// <param name="control"></param>
    /// <param name="dataContextGetterAsync"></param>
    /// <param name="exceptionCallback"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static async ValueTask<bool> LoadDataContextAsync(
        this ContentControl control,
        Func<CancellationToken, Task<object>> dataContextGetterAsync,
        Action<Exception>? exceptionCallback = null,
        CancellationToken cancellation = default
    )
    {
        //原始内容是否是框架元素   
        var tmpContent = control.Content;

        //替换为加载中
        control.Content = new LoadingStateView("正在加载页面");

        try
        {
            control.DataContext = await dataContextGetterAsync(cancellation);
            control.Content = tmpContent;

            return true;
        }
        catch (Exception ex)
        {
            control.Content = new MessageStateView("发生错误", ex.Message);
            control.DataContext = null;

            exceptionCallback?.Invoke(ex);

            return false;
        }
    }

    /// <summary>
    ///     释放DataContext
    /// </summary>
    /// <param name="element"></param>
    public static object? ReleaseDataContext(this FrameworkElement element)
    {
        var dataContext = element.DataContext;
        element.DataContext = null;

        return dataContext;
    }
}