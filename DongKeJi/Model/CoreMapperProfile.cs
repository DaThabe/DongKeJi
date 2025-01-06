using AutoMapper;
using DongKeJi.Model.Entity;
using DongKeJi.ViewModel.User;

namespace DongKeJi.Model;

internal class CoreMapperProfile : Profile
{
    public CoreMapperProfile()
    {
        CreateMap<UserEntity, UserViewModel>().ReverseMap();
    }
}