using AutoMapper;
using RedditAPI.Entities;
using RedditAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedditAPI.MappingProfiles
{
    public class RedditMappingProfile : Profile
    {
        public RedditMappingProfile()
        {
            CreateMap<History, HistoryDto>();
            CreateMap<string, ImageDto>()
                .ForMember(m => m.Url, u => u.MapFrom(s => s));
        }
    }
}
