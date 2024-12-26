using AutoMapper;
using DongKeJi.Model.Entity;
using DongKeJi.ViewModel.User;

namespace DongKeJi.Model;

internal class DongKeJiMapperProfile : Profile
{
    public DongKeJiMapperProfile()
    {
        CreateMap<UserEntity, UserViewModel>().ReverseMap();
    }
}