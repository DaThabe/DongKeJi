using AutoMapper;
using DongKeJi.Exceptions;
using DongKeJi.Inject;
using DongKeJi.WebView.Model;
using DongKeJi.WebView.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.WebView.Service;

/// <summary>
/// 工具服务
/// </summary>
public interface IWebViewService
{
    /// <summary>
    /// 导航到网址
    /// </summary>
    ValueTask NavigationAsync(Uri uri);

    /// <summary>
    /// 添加页面
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    ValueTask AddPageAsync(PageViewModel page);

    /// <summary>
    /// 删除页面
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    ValueTask RemovePageAsync(PageViewModel page);
}


[Inject(ServiceLifetime.Singleton, typeof(IWebViewService))]
internal class WebViewService(
    IWebViewModule module, 
    WebViewDbContext dbContext,
    IMapper mapper) : IWebViewService
{
    public async ValueTask NavigationAsync(Uri uri)
    {
        try
        {
            var pageItem = await dbContext.Page
                .FirstOrDefaultAsync(x => x.Source == uri);

            pageItem = DatabaseException.ThrowIfEntityNotFound(pageItem);

            module.CurrentPage = mapper.Map<PageViewModel>(pageItem);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"导航至页面时发生错误\n网址: {uri}", ex);
        }
    }

    public ValueTask AddPageAsync(PageViewModel page)
    {
        //TODO: 添加页面
        return ValueTask.CompletedTask;
    }

    public ValueTask RemovePageAsync(PageViewModel page)
    {
        //TODO: 删除页面
        return ValueTask.CompletedTask;
    }
}