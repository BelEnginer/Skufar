using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

public class TradeRequestMappingProfile: Profile
{
    public TradeRequestMappingProfile()
    {
        CreateMap<TradeRequestDto, TradeRequest>()
            .ReverseMap();
        CreateMap<TradeRequestPostDto, TradeRequest>()
            .ReverseMap();
    }
}