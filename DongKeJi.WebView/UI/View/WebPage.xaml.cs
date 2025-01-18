using System.Windows.Controls;
using DongKeJi.Inject;
using DongKeJi.WebView.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.WebView.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class WebPage
{
    private readonly IServiceProvider _services;

    public WebPage(IServiceProvider services)
    {
        _services = services;
        InitializeComponent();
    }

    protected override ValueTask OnNavigatedToAsync(CancellationToken cancellation = default)
    {
        var vm = _services.GetRequiredService<WebPageViewModel>();
        vm.PageCollection.Add(new PageViewModel(null, "BiliBili", new Uri("https://www.bilibili.com/")));
        vm.PageCollection.Add(new PageViewModel(null, "微博", new Uri("https://weibo.com/")));
        vm.PageCollection.Add(new PageViewModel(null, "小红书", new Uri("https://www.xiaohongshu.com/explore/")));

        DataContext = vm;

        return base.OnNavigatedToAsync(cancellation);
    }

    protected override ValueTask OnNavigatedFromAsync(CancellationToken cancellation = default)
    {
        WebView2.Source = EmptySource;
        DataContext = null;
        return base.OnNavigatedFromAsync(cancellation);
    }

    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not WebPageViewModel vm) return;
        if (args.SelectedItem is PageViewModel page) vm.SelectedPage = page;
    }


    private static Uri EmptySource { get; } = new Uri("about:blank");
}