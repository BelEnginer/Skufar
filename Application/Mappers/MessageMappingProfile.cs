using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageDto>()
            .ReverseMap();

        CreateMap<Message, MessagePostDto>()
            .ReverseMap();
    }
}
