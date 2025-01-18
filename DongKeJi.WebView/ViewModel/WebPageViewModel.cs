using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DongKeJi.Inject;
using DongKeJi.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DongKeJi.WebView.ViewModel;


[Inject(ServiceLifetime.Transient)]
public partial class WebPageViewModel : ObservableViewModel
{
    /// <summary>
    /// 选中的网页
    /// </summary>
    [ObservableProperty] 
    private PageViewModel? _selectedPage;

    /// <summary>
    /// 网页集合
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<PageViewModel> _pageCollection = [];


    partial void OnSelectedPageChanged(PageViewModel? value)
    {
        if (value is null) SelectedPage = Empty;
    }



    public static PageViewModel Empty { get; } = new(null, "", new Uri("about:blank"));
}