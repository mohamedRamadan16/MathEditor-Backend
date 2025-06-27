using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;
using Api.Application.Documents.DTOs;
using AutoMapper;

namespace Api.Application.Documents.Queries
{
    public class GetDocumentByHandleQuery : IRequest<DocumentResponseDto?>
    {
        public string Handle { get; }
        public GetDocumentByHandleQuery(string handle) => Handle = handle;
    }
}
