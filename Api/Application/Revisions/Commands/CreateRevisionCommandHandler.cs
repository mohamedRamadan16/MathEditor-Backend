using MediatR;
using Api.Application.Common.Interfaces;
using Api.Application.Revisions.DTOs;
using Api.Domain.Entities;
using AutoMapper;

namespace Api.Application.Revisions.Commands;

public class CreateRevisionCommandHandler : IRequestHandler<CreateRevisionCommand, RevisionResponseDto?>
{
    private readonly IRevisionRepository _revRepo;
    private readonly IDocumentRepository _docRepo;
    private readonly IMapper _mapper;
    public CreateRevisionCommandHandler(IRevisionRepository revRepo, IDocumentRepository docRepo, IMapper mapper)
    {
        _revRepo = revRepo;
        _docRepo = docRepo;
        _mapper = mapper;
    }
    public async Task<RevisionResponseDto?> Handle(CreateRevisionCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var doc = await _docRepo.FindByIdAsync(dto.DocumentId);
        if (doc == null)
            return null;
        var userEmail = request.UserEmail?.Trim().ToLowerInvariant();
        var isCoauthor = doc.Coauthors != null && doc.Coauthors.Any(ca => ca.UserEmail.ToLower() == userEmail);
        if (doc.AuthorId != request.UserId && !isCoauthor)
            return null;

        var rev = _mapper.Map<Revision>(dto);
        rev.Id = dto.Id != null && dto.Id != Guid.Empty ? dto.Id.Value : Guid.NewGuid();
        rev.AuthorId = request.UserId;
        rev.CreatedAt = dto.CreatedAt ?? DateTime.UtcNow;

        var created = await _revRepo.CreateAsync(rev);
        doc.Head = created.Id;
        doc.UpdatedAt = DateTime.UtcNow;

        await _docRepo.UpdateAsync(doc);
        var response = _mapper.Map<RevisionResponseDto>(created);
        // Optionally set author info if not mapped
        if (response.Author == null)
            response.Author = new AuthorDto { Id = request.UserId, Name = request.UserName ?? string.Empty };
        return response;
    }
}
