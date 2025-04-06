using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<ReviewDto, Review>()
            .ReverseMap();
        CreateMap<ReviewPostDto, Review>()
            .ReverseMap();
    }
}