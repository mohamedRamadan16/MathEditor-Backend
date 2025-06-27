using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Api.Application.Documents.DTOs;
using Api.Application.Common.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Api.Application.Documents.Commands
{
    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, DocumentResponseDto?>
    {
        private readonly IDocumentRepository _repo;
        private readonly IMapper _mapper;
        public CreateDocumentCommandHandler(IDocumentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<DocumentResponseDto?> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var userId = request.UserId;
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Handle))
                return null;
            var doc = _mapper.Map<Document>(dto);
            doc.Id = Guid.NewGuid();
            doc.AuthorId = userId;
            doc.CreatedAt = dto.CreatedAt ?? DateTime.UtcNow;
            doc.UpdatedAt = dto.UpdatedAt ?? DateTime.UtcNow;
            
            if (doc.Head == Guid.Empty && doc.Revisions != null && doc.Revisions.Count > 0)
                doc.Head = doc.Revisions.FirstOrDefault()?.Id ?? Guid.Empty;

            var firstRev = doc.Revisions?.FirstOrDefault();
            if (firstRev != null)
            {
                firstRev.AuthorId = userId;
                firstRev.DocumentId = doc.Id;
            }
            if (doc.Coauthors != null)
                foreach (var ca in doc.Coauthors)
                    ca.DocumentId = doc.Id;

            await _repo.CreateAsync(doc);
            var created = await _repo.FindByIdAsync(doc.Id);
            
            return created == null ? null : _mapper.Map<DocumentResponseDto>(created);
        }
    }
}
