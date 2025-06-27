using MediatR;
using Api.Application.Common.Interfaces;
using Api.Application.Documents.DTOs;
using AutoMapper;

namespace Api.Application.Documents.Queries;

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentResponseDto?>
{
    private readonly IDocumentRepository _repo;
    private readonly IMapper _mapper;
    public GetDocumentByIdQueryHandler(IDocumentRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    public async Task<DocumentResponseDto?> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var doc = await _repo.FindByIdAsync(request.Id);
        return doc == null ? null : _mapper.Map<DocumentResponseDto>(doc);
    }
}
