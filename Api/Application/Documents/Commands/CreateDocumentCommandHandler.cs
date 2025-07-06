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

            // Validate initial revision Lexical state structure
            if (dto.InitialRevision?.Data?.Root == null || dto.InitialRevision.Data.Root.Children == null)
            {
                // Invalid initial revision Lexical state: return null so controller can return 400 Bad Request
                return null;
            }

            var doc = _mapper.Map<Document>(dto);
            doc.Id = Guid.NewGuid();
            doc.AuthorId = userId;
            doc.CreatedAt = DateTime.UtcNow;
            doc.UpdatedAt = DateTime.UtcNow;
            
            if (doc.Head == Guid.Empty && doc.Revisions != null && doc.Revisions.Count > 0)
                doc.Head = doc.Revisions.FirstOrDefault()?.Id ?? Guid.Empty;

            var firstRev = doc.Revisions?.FirstOrDefault();
            if (firstRev != null)
            {
                if (firstRev.Id == Guid.Empty)
                    firstRev.Id = Guid.NewGuid();
                doc.Head = firstRev.Id;
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
