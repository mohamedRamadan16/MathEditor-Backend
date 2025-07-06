using MediatR;
using Api.Application.Common.Interfaces;
using Api.Application.Revisions.DTOs;
using Api.Domain.Entities;
using AutoMapper;
using System.Text.Json;

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
        
        // Validate Lexical state structure
        if (dto.Data?.Root == null || dto.Data.Root.Children == null)
        {
            // Invalid Lexical state: return null so controller can return 400 Bad Request
            return null;
        }

        var doc = await _docRepo.FindByIdAsync(dto.DocumentId);
        if (doc == null)
            return null;
        
        var userEmail = request.UserEmail?.Trim().ToLowerInvariant();
        var isCoauthor = doc.Coauthors != null && doc.Coauthors.Any(ca => ca.UserEmail.ToLower() == userEmail);
        
        if (doc.AuthorId != request.UserId && !isCoauthor)
            return null;

        // Create revision entity
        var rev = new Revision
        {
            DocumentId = dto.DocumentId,
            Data = dto.GetDataAsJson(), // Convert structured data to JSON string for storage
            AuthorId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

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
