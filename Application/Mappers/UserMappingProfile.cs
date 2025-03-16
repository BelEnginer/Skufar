using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using AutoMapper;
using Domain.Entites;

namespace Application.Mappers;

public class UserMappingProfile: Profile
{
    public UserMappingProfile()
    {
    CreateMap<UserDto, User>()
      .ReverseMap();
    CreateMap<UserUpdateDto, User>()
      .ReverseMap();
    CreateMap<UserRegisterDto, User>()
      .ReverseMap();
      
    }   
}