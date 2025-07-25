using AutoMapper;
using Api.Domain.Entities;
using Api.Application.Documents.DTOs;

namespace Api.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentResponseDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Coauthors, opt => opt.MapFrom(src => src.Coauthors))
                .ForMember(dest => dest.Revisions, opt => opt.MapFrom(src => src.Revisions));
            CreateMap<DocumentCoauthor, DocumentUserResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Handle, opt => opt.MapFrom(src => src.User.Handle))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.Image));
            CreateMap<User, DocumentUserResponseDto>();
            CreateMap<Revision, DocumentRevisionResponseDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
            CreateMap<DocumentCreateDto, Document>()
                .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
                .ForMember(dest => dest.Collab, opt => opt.MapFrom(src => src.Collab))
                .ForMember(dest => dest.Private, opt => opt.MapFrom(src => src.Private))
                .ForMember(dest => dest.Coauthors, opt => opt.MapFrom(src => src.Coauthors != null ? src.Coauthors.Select(email => new DocumentCoauthor { UserEmail = email.Trim().ToLowerInvariant(), CreatedAt = DateTime.UtcNow }).ToList() : new List<DocumentCoauthor>()))
                .ForMember(dest => dest.Revisions, opt => opt.MapFrom(src => src.InitialRevision != null ? new List<Revision> { new Revision {
                    Data = src.InitialRevision.GetDataAsJson()
                }} : new List<Revision>()))
                .ForMember(dest => dest.Head, opt => opt.MapFrom(src => Guid.Empty));
        }
    }
}
