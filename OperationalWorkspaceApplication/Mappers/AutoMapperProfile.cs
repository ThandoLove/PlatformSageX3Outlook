using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace OperationalWorkspaceApplication.Mappers;


public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Entity to DTO (Read operations)
        CreateMap<SalesOrder, SalesOrderDto>();
        CreateMap<SalesOrderLine, SalesOrderLineDto>();
        CreateMap<Client, ClientDto>();
        CreateMap<SalesOrder, OpenOrderDto>();

        // DTO to Entity (Write operations)
        CreateMap<CreateOrderDto, SalesOrder>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<OrderDto, SalesOrder>();

        
    }
}
