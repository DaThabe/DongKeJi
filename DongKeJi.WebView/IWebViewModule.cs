using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using DongKeJi.WebView.Model;
using DongKeJi.WebView.Service;
using DongKeJi.WebView.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DongKeJi.WebView;


/// <summary>
/// 办公模块
/// </summary>
public interface IWebViewModule : IModule
{
    /// <summary>
    /// 当前页面
    /// </summary>
    PageViewModel? CurrentPage { get; internal set; }
}

/// <summary>
/// 网页视图模块
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IWebViewModule))]
public class WebViewModule : ObservableViewModel, IWebViewModule
{
    public static IModuleInfo Info { get; }= new ModuleInfo
    {
        Id = Guid.NewGuid(),
        Name = "DongKeJi.WebView",
        Version = new Version(0, 0, 1),
        Title = "网页",
        Developers = ["DaThabe"],
        Describe = """
                   ### 网页模块
                   - 提供网页收藏和浏览功能
                   """,
        CreatedAt = new DateTime(2025, 1, 18),
        ReleaseDate = new DateTime(2025, 1, 18),
        Dependencies = typeof(WebViewModule).Assembly.GetReferencedAssemblies()
    };

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //反射注入
            services.AddAutoInject<WebViewModule>();
            //启动后业务
            services.AddHostedService<HostedService>();
            
            //数据库
            services.AddDbContext<WebViewDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WebViewMapperProfile));
        });
    }

    public PageViewModel? CurrentPage { get; set; }
}