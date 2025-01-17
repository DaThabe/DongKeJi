using AutoMapper;
using DongKeJi.Core.Model.Entity;
using DongKeJi.Core.ViewModel.User;
using UserViewModel = DongKeJi.Core.ViewModel.User.UserViewModel;

namespace DongKeJi.Core.Model;

internal class CoreMapperProfile : Profile
{
    public CoreMapperProfile()
    {
        CreateMap<UserEntity, UserViewModel>().ReverseMap();
    }
}