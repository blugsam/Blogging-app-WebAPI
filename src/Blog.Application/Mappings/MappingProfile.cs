using AutoMapper;
using Blog.Application.Contracts.PostDtos;
using Blog.Application.Contracts.TagDtos;
using Blog.Domain.Entities;

namespace Blog.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag)));

            CreateMap<CreatePostDto, Post>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.PostTags, opt => opt.Ignore());

            CreateMap<UpdatePostDto, Post>()
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.PostTags, opt => opt.Ignore());

            CreateMap<Tag, TagDto>();
        }
    }
}