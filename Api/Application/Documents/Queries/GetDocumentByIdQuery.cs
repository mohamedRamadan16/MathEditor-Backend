using Api.Application.Documents.DTOs;
using MediatR;

namespace Api.Application.Documents.Queries;

public class GetDocumentByIdQuery : IRequest<DocumentResponseDto?>
{
    public Guid Id { get; }
    public GetDocumentByIdQuery(Guid id) => Id = id;
}
