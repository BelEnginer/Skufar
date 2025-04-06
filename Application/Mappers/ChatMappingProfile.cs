using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<Chat, ChatDto>()
                .ForMember(dest => dest.LastMessage,
                    opt => opt.MapFrom(src => src.Messages
                        .OrderByDescending(m => m.Date)
                        .Select(m => m.Content)
                        .FirstOrDefault()))
                .ReverseMap();

            
            CreateMap<Chat, ChatPostDto>()
                .ReverseMap();
        }
    }