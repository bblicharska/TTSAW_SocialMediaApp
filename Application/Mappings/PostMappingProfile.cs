using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class PostMappingProfile : Profile
    {
        public PostMappingProfile()
        {
            // Mapowanie postów
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes));

            CreateMap<CreatePostDto, Post>();
            CreateMap<UpdatePostDto, Post>();

            // Mapowanie komentarzy
            CreateMap<Comment, CommentDto>();
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<UpdateCommentDto, Comment>();

            // Mapowanie polubień
            CreateMap<Like, LikeDto>();
            CreateMap<CreateLikeDto, Like>();
        }
    }

}
