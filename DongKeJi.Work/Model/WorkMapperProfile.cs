using AutoMapper;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Consume;
using DongKeJi.Work.ViewModel.Common.Customer;
using DongKeJi.Work.ViewModel.Common.Order;
using DongKeJi.Work.ViewModel.Common.Staff;
using ConsumeCountingViewModel = DongKeJi.Work.ViewModel.Consume.ConsumeCountingViewModel;
using ConsumeMixingViewModel = DongKeJi.Work.ViewModel.Consume.ConsumeMixingViewModel;
using ConsumeTimingViewModel = DongKeJi.Work.ViewModel.Consume.ConsumeTimingViewModel;
using ConsumeViewModel = DongKeJi.Work.ViewModel.Consume.ConsumeViewModel;
using CustomerViewModel = DongKeJi.Work.ViewModel.Customer.CustomerViewModel;
using OrderCountingViewModel = DongKeJi.Work.ViewModel.Order.OrderCountingViewModel;
using OrderMixingViewModel = DongKeJi.Work.ViewModel.Order.OrderMixingViewModel;
using OrderTimingViewModel = DongKeJi.Work.ViewModel.Order.OrderTimingViewModel;
using OrderViewModel = DongKeJi.Work.ViewModel.Order.OrderViewModel;
using StaffPositionViewModel = DongKeJi.Work.ViewModel.Staff.StaffPositionViewModel;
using StaffViewModel = DongKeJi.Work.ViewModel.Staff.StaffViewModel;

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