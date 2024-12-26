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

internal class PerformanceRecordMapperProfile : Profile
{
    public PerformanceRecordMapperProfile()
    {
        //员工
        CreateMap<StaffEntity, StaffViewModel>().ReverseMap();
        CreateMap<StaffPositionEntity, StaffPositionViewModel>().ReverseMap();

        //机构
        CreateMap<CustomerEntity, CustomerViewModel>().ReverseMap();

        //方案
        CreateMap<TimingOrderEntity, TimingOrderViewModel>().ReverseMap();
        CreateMap<CountingOrderEntity, CountingOrderViewModel>().ReverseMap();
        CreateMap<MixingOrderEntity, MixingOrderViewModel>().ReverseMap();

        //划扣
        CreateMap<TimingConsumeEntity, TimingConsumeViewModel>().ReverseMap();
        CreateMap<CountingConsumeEntity, CountingConsumeViewModel>().ReverseMap();
        CreateMap<MixingConsumeEntity, MixingConsumeViewModel>().ReverseMap();


        //订单派生类关联
        CreateMap<OrderEntity, OrderViewModel>()
            .Include<TimingOrderEntity, TimingOrderViewModel>()
            .Include<CountingOrderEntity, CountingOrderViewModel>()
            .Include<MixingOrderEntity, MixingOrderViewModel>()
            .ReverseMap();

        //划扣派生类关联
        CreateMap<ConsumeEntity, ConsumeViewModel>()
            .Include<TimingConsumeEntity, TimingConsumeViewModel>()
            .Include<CountingConsumeEntity, CountingConsumeViewModel>()
            .Include<MixingConsumeEntity, MixingConsumeViewModel>()
            .ReverseMap();
    }
}