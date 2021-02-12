using AutoMapper;
using NETCoreTemplate.Entity.Entities;
using NETCoreTemplate.Entity.Models;

namespace NETCoreTemplate.WebAPI.Extensions
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            //CreateMap<DemoObject, DemoObjectModel>();
            //CreateMap<DemoObjectModel, DemoObject>();
        }
    }
}
