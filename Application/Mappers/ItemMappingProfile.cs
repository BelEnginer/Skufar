using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

public class ItemMappingProfile : Profile
{
   public ItemMappingProfile()
   {
      CreateMap<Item, ItemDto>()
         .ForMember(dest => dest.PreviewImagePath, opt => opt.MapFrom(src => src.PreviewImagePath))
         .ForMember(dest => dest.ImagePaths, opt => opt.Ignore())
         .ReverseMap();

      CreateMap<ItemUpdateDto, Item>()
         .ForMember(dest => dest.Category, 
            opt => opt.Ignore()) 
         .ForMember(dest => dest.Owner, 
            opt => opt.Ignore()) 
         .ReverseMap();

   CreateMap<ItemPostDto, Item>()
    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => 
        src.Images.Select(img => new ItemImage { ImagePath = "" }).ToList()))
    .ReverseMap();


   }
}
