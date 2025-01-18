using AutoMapper;
using DongKeJi.WebView.Model.Entity;
using DongKeJi.WebView.ViewModel;

namespace DongKeJi.WebView.Model;

internal class WebViewMapperProfile : Profile
{
    public WebViewMapperProfile()
    {
        //网页
        CreateMap<PageEntity, PageViewModel>().ReverseMap();
    }
}