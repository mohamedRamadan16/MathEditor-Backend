using AutoMapper;
using Api.Domain.Entities;
using Api.Application.Documents.DTOs;
using Api.Application.Users.DTOs;

namespace Api.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, DocumentUserResponseDto>();
            CreateMap<User, UserResponseDto>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}
