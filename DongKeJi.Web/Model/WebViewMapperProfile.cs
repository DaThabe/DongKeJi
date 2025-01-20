using AutoMapper;
using DongKeJi.Web.Model.Entity;
using DongKeJi.Web.ViewModel;

namespace DongKeJi.Web.Model;

internal class WebViewMapperProfile : Profile
{
    public WebViewMapperProfile()
    {
        //网页
        CreateMap<PageEntity, PageViewModel>().ReverseMap();
    }
}