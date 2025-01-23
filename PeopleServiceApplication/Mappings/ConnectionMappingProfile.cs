using AutoMapper;
using PeopleServiceApplication.Dto;
using PeopleServiceDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ConnectionMappingProfile : Profile
{
    public ConnectionMappingProfile()
    {

        CreateMap<Connection, ConnectionDto>();

        CreateMap<CreateConnectionDto, Connection>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateConnectionDto, Connection>()
            .ForMember(dest => dest.FriendId, opt => opt.MapFrom(src => src.FriendId));
    }
}