using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Application.Common.Interfaces;
using MediatR;
using Api.Application.Documents.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Api.Application.Documents.Commands
{
    public class CreateDocumentCommand : IRequest<DocumentResponseDto?>
    {
        public DocumentCreateDto Dto { get; }
        public Guid UserId { get; }
        public CreateDocumentCommand(DocumentCreateDto dto, Guid userId)
        {
            Dto = dto;
            UserId = userId;
        }
    }
}
