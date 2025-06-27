using MediatR;
using Api.Application.Documents.DTOs;

namespace Api.Application.Documents.Queries
{
    public class GetAllDocumentsQuery : IRequest<GetAllDocumentsResultDto>
    {
        public int Page { get; }
        public int PageSize { get; }
        public GetAllDocumentsQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
