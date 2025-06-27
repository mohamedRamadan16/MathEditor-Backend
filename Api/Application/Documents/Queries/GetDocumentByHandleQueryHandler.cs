using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Api.Application.Common.Interfaces;
using Api.Application.Documents.DTOs;
using AutoMapper;

namespace Api.Application.Documents.Queries
{
    public class GetDocumentByHandleQueryHandler : IRequestHandler<GetDocumentByHandleQuery, DocumentResponseDto?>
    {
        private readonly IDocumentRepository _repo;
        private readonly IMapper _mapper;
        public GetDocumentByHandleQueryHandler(IDocumentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<DocumentResponseDto?> Handle(GetDocumentByHandleQuery request, CancellationToken cancellationToken)
        {
            var doc = await _repo.FindByHandleAsync(request.Handle);
            return doc == null ? null : _mapper.Map<DocumentResponseDto>(doc);
        }
    }
}
