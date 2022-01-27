using AutoMapper;
using RedditAPI.Entities;
using RedditAPI.Models;

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
