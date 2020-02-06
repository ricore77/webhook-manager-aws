using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookManager.Dto;
using WebhookManager.Model;

namespace WebhookManager.Api.Extensions
{
    public class DefaultProfile : AutoMapper.Profile
    {
        public DefaultProfile()
        {
            CreateMap<Application, ApplicationCreateRequest>().ReverseMap();
            CreateMap<Event, EventDto>()
                    .ForMember(dest =>
                        dest.Name,
                        opt => opt.MapFrom(src => src.EventName))                   
                    .ReverseMap();
        }
    }
}
