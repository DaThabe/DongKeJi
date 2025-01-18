using AutoMapper;
using DongKeJi.Database;
using DongKeJi.Entity;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using DongKeJi.WebView.Model;
using DongKeJi.WebView.Model.Entity;
using DongKeJi.WebView.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.WebView;

/// <summary>
/// 办公模块数据库
/// </summary>
public interface IWebViewDatabase
{
    internal IServiceProvider ServiceProvider { get; }
}

[Inject(ServiceLifetime.Singleton, typeof(IWebViewDatabase))]
internal class WebViewDatabase(IServiceProvider services) : IWebViewDatabase
{
    public IServiceProvider ServiceProvider { get; } = services;
}

public static class WebViewDatabaseExtensions
{
    private static IAutoUpdateBuilder<TViewModel> AutoUpdate<TEntity, TViewModel>(this IWebViewDatabase db, TViewModel viewModel)
        where TEntity : EntityBase
        where TViewModel : IEntityViewModel
    {
        var mapper = db.ServiceProvider.GetRequiredService<IMapper>();
        var dbContext = db.ServiceProvider.GetRequiredService<WebViewDbContext>();

        return dbContext.AutoUpdate<TEntity, TViewModel>(viewModel, mapper);
    }

    public static IAutoUpdateBuilder<PageViewModel> AutoUpdate(this IWebViewDatabase db, PageViewModel vm)
        => db.AutoUpdate<PageEntity, PageViewModel>(vm);
}