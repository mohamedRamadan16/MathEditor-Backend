using AutoMapper;
using Api.Domain.Entities;
using Api.Application.Revisions.DTOs;

namespace Api.Application.Mappings
{
    public class RevisionProfile : Profile
    {
        public RevisionProfile()
        {
            CreateMap<Revision, RevisionResponseDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
            CreateMap<User, AuthorDto>();
            CreateMap<CreateRevisionDto, Revision>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Document, opt => opt.Ignore());
        }
    }
}
