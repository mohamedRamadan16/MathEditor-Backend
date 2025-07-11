using MediatR;
using Api.Application.Documents.DTOs;

namespace Api.Application.Documents.Queries
{
    public class GetUserDocumentsQuery : IRequest<GetAllDocumentsResultDto>
    {
        public Guid UserId { get; }
        public int Page { get; }
        public int PageSize { get; }
        
        public GetUserDocumentsQuery(Guid userId, int page, int pageSize)
        {
            UserId = userId;
            Page = page;
            PageSize = pageSize;
        }
    }
}
