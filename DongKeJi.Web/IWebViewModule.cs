using DongKeJi.Inject;
using DongKeJi.Module;
using DongKeJi.ViewModel;
using DongKeJi.Web.Model;
using DongKeJi.Web.Service;
using DongKeJi.Web.ViewModel;

namespace DongKeJi.Web;


/// <summary>
/// �칫ģ��
/// </summary>
public interface IWebViewModule : IModule
{
    /// <summary>
    /// ��ǰҳ��
    /// </summary>
    PageViewModel? CurrentPage { get; internal set; }
}

/// <summary>
/// ��ҳ��ͼģ��
/// </summary>
[Inject(ServiceLifetime.Singleton, typeof(IWebViewModule))]
public class WebModule : ObservableViewModel, IWebViewModule
{
    public static IModuleInfo Info { get; } = ModuleInfo.CreateFromAssembly<WebModule>(config =>
    {
        config.Name = "DongKeJi.Web";
        config.Title = "��ҳ";
        config.Developers = ["DaThabe"];
        config.CreatedAt = new DateTime(2025, 1, 18);
        config.ReleaseDate = new DateTime(2025, 1, 18);
    });

    public static void Configure(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //����ע��
            services.AddAutoInject<WebModule>();
            //������ҵ��
            services.AddHostedService<HostedService>();
            
            //���ݿ�
            services.AddDbContext<WebViewDbContext>();
            //AutoMapper
            services.AddAutoMapper(typeof(WebViewMapperProfile));
        });
    }

    public PageViewModel? CurrentPage { get; set; }
}