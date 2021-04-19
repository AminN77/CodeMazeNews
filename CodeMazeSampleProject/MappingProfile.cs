using AutoMapper;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;

namespace CodeMazeSampleProject
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<News, NewsDto>();
            CreateMap<CategoryForCreationDto, Category>();
            CreateMap<NewsForCreationDto, News>();
            CreateMap<NewsForUpdateDto, News>().ReverseMap();
            CreateMap<CategoryForUpdateDto, Category>();
        }
    }
}