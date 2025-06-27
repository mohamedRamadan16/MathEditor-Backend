using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Api.Application.Common.Interfaces;
using Api.Application.Documents.DTOs;
using System;
using System.Linq;
using AutoMapper;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentCommand : IRequest<DocumentResponseDto?>
    {
        public DocumentUpdateDto Dto { get; }
        public Guid UserId { get; }
        public string? UserEmail { get; }
        public UpdateDocumentCommand(DocumentUpdateDto dto, Guid userId, string? userEmail)
        {
            Dto = dto;
            UserId = userId;
            UserEmail = userEmail;
        }
    }
}
