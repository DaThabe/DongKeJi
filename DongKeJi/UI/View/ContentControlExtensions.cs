//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Threading;

//namespace DongKeJi.UI.View;

//public static class ContentControlExtensions
//{
//    /// <summary>
//    ///     异步加载DataContext  (传入异步过程
//    /// </summary>
//    /// <param name="control"></param>
//    /// <param name="dataContextGetterAsync"></param>
//    /// <param name="exceptionCallback"></param>
//    /// <param name="cancellation"></param>
//    /// <returns></returns>
//    public static async ValueTask<bool> LoadDataContextAsync(
//        this ContentControl control,
//        Func<CancellationToken, Task<object>> dataContextGetterAsync,
//        Action<Exception>? exceptionCallback = null,
//        CancellationToken cancellation = default
//    )
//    {
//        try
//        {
//            //原始内容
//            var tmpContent = (FrameworkElement)control.Content;

//            control.Content = new LoadingStateView("正在加载页面");
//            control.DataContext = await dataContextGetterAsync(cancellation);
//            control.Content = tmpContent;

//            return true;
//        }
//        catch (Exception ex)
//        {
//            control.Content = new MessageStateView("发生错误", ex.Message);
//            control.DataContext = null;

//            exceptionCallback?.Invoke(ex);

//            return false;
//        }
//    }

//    /// <summary>
//    ///     释放DataContext
//    /// </summary>
//    /// <param name="element"></param>
//    public static object? ReleaseDataContext(this FrameworkElement element)
//    {
//        var dataContext = element.DataContext;
//        element.DataContext = null;

//        return dataContext;
//    }
//}