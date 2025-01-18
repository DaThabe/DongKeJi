namespace DongKeJi.WebView.Model;


/// <summary>
/// 网页元素
/// </summary>
public interface IPage
{
    /// <summary>
    /// 标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// 页面网址
    /// </summary>
    Uri Source { get; set; }
}