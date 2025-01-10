using AutoMapper;
using DongKeJi.Work.Model.Entity.Consume;
using DongKeJi.Work.Model.Entity.Customer;
using DongKeJi.Work.Model.Entity.Order;
using DongKeJi.Work.Model.Entity.Staff;
using DongKeJi.Work.ViewModel.Common.Consume;
using DongKeJi.Work.ViewModel.Common.Customer;
using DongKeJi.Work.ViewModel.Common.Order;
using DongKeJi.Work.ViewModel.Common.Staff;

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
        CreateMap<TimingOrderEntity, OrderTimingViewModel>().ReverseMap();
        CreateMap<CountingOrderEntity, OrderCountingViewModel>().ReverseMap();
        CreateMap<MixingOrderEntity, OrderMixingViewModel>().ReverseMap();

        //划扣
        CreateMap<TimingConsumeEntity, ConsumeTimingViewModel>().ReverseMap();
        CreateMap<CountingConsumeEntity, ConsumeCountingViewModel>().ReverseMap();
        CreateMap<MixingConsumeEntity, ConsumeMixingViewModel>().ReverseMap();


        //订单派生类关联
        CreateMap<OrderEntity, OrderViewModel>()
            .Include<TimingOrderEntity, OrderTimingViewModel>()
            .Include<CountingOrderEntity, OrderCountingViewModel>()
            .Include<MixingOrderEntity, OrderMixingViewModel>()
            .ReverseMap();

        //划扣派生类关联
        CreateMap<ConsumeEntity, ConsumeViewModel>()
            .Include<TimingConsumeEntity, ConsumeTimingViewModel>()
            .Include<CountingConsumeEntity, ConsumeCountingViewModel>()
            .Include<MixingConsumeEntity, ConsumeMixingViewModel>()
            .ReverseMap();
    }
}