using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CategoryDto,Category>()
            .ReverseMap();
    }
}