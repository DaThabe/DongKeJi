using DongKeJi.Inject;
using DongKeJi.Web.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;

namespace DongKeJi.Web.UI.View;


[Inject(ServiceLifetime.Singleton)]
public partial class WebPage : INavigationAware
{
    public WebPage(WebPageViewModel vm)
    {
        InitializeComponent();

        vm.PageCollection.Add(new PageViewModel(null, "BiliBili", new Uri("https://www.bilibili.com/")));
        vm.PageCollection.Add(new PageViewModel(null, "微博", new Uri("https://weibo.com/")));
        vm.PageCollection.Add(new PageViewModel(null, "小红书", new Uri("https://www.xiaohongshu.com/explore/")));

        DataContext = vm;
    }


    public void OnNavigatedTo()
    {

    }

    public void OnNavigatedFrom()
    {
        if (DataContext is WebPageViewModel vm)
        {
            vm.SelectedPage = null;
        }
    }


    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (DataContext is not WebPageViewModel vm) return;
        if (args.SelectedItem is PageViewModel page) vm.SelectedPage = page;
    }

    
}