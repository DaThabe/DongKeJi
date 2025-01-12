using AutoMapper;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Consume;
using DongKeJi.Work.ViewModel.Customer;
using DongKeJi.Work.ViewModel.Order;
using DongKeJi.Work.ViewModel.Staff;

namespace DongKeJi.Work.Model;

internal class WorkMapperProfile : Profile
{
    public WorkMapperProfile()
    {
        //员工
        CreateMap<StaffEntity, StaffViewModel>().ReverseMap();
        CreateMap<StaffPositionEntity, StaffPositionViewModel>().ReverseMap();

        //机构
        CreateMap<CustomerEntity, CustomerViewModel>().ReverseMap();

        //方案
        CreateMap<OrderTimingEntity, OrderTimingViewModel>().ReverseMap();
        CreateMap<OrderCountingEntity, OrderCountingViewModel>().ReverseMap();
        CreateMap<OrderMixingEntity, OrderMixingViewModel>().ReverseMap();

        //划扣
        CreateMap<ConsumeTimingEntity, ConsumeTimingViewModel>().ReverseMap();
        CreateMap<ConsumeCountingEntity, ConsumeCountingViewModel>().ReverseMap();
        CreateMap<ConsumeMixingEntity, ConsumeMixingViewModel>().ReverseMap();


        //订单派生类关联
        CreateMap<OrderEntity, OrderViewModel>()
            .Include<OrderTimingEntity, OrderTimingViewModel>()
            .Include<OrderCountingEntity, OrderCountingViewModel>()
            .Include<OrderMixingEntity, OrderMixingViewModel>()
            .ReverseMap();

        //划扣派生类关联
        CreateMap<ConsumeEntity, ConsumeViewModel>()
            .Include<ConsumeTimingEntity, ConsumeTimingViewModel>()
            .Include<ConsumeCountingEntity, ConsumeCountingViewModel>()
            .Include<ConsumeMixingEntity, ConsumeMixingViewModel>()
            .ReverseMap();
    }
}