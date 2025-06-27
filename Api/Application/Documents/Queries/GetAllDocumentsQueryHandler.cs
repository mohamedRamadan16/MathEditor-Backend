using MediatR;
using Api.Application.Documents.DTOs;
using Api.Application.Common.Interfaces;
using AutoMapper;

namespace Api.Application.Documents.Queries
{
    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, GetAllDocumentsResultDto>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }
        public async Task<GetAllDocumentsResultDto> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var (docs, totalCount) = await _documentRepository.GetPublishedPagedAsync(page, pageSize, tracked: false);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var result = _mapper.Map<List<DocumentResponseDto>>(docs);
            
            return new GetAllDocumentsResultDto
            {
                Items = result,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
