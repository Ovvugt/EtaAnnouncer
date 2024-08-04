using AutoMapper;
using EtaAnnouncer.Contracts;
using EtaAnnouncer.Models;

namespace EtaAnnouncer
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Member, MemberDto>();
            CreateMap<Home, HomeDto>();
        }
    }
}
