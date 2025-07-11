using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Api.Application.Documents.DTOs;
using Api.Application.Common.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using System;
using System.Linq;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, DocumentResponseDto?>
    {
        private readonly IDocumentRepository _repo;
        private readonly IMapper _mapper;
        public UpdateDocumentCommandHandler(IDocumentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<DocumentResponseDto?> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var userId = request.UserId;
            var userEmail = request.UserEmail;
            if (dto == null || dto.Id == Guid.Empty)
                return null;
            var doc = await _repo.FindByIdAsync(dto.Id);
            if (doc == null)
                return null;
            // Only author or coauthor can update
            var isCoauthor = doc.Coauthors.Any(ca =>
                (ca.User != null && ca.User.Id == userId) ||
                (!string.IsNullOrEmpty(userEmail) && ca.UserEmail == userEmail)
            );
            if (doc.AuthorId != userId && !isCoauthor)
                return null; // Forbidden
            
            // Only update fields that are provided (not null)
            if (dto.Name != null)
                doc.Name = dto.Name;
            if (dto.Handle != null)
                doc.Handle = dto.Handle;
            if (dto.Published.HasValue)
                doc.Published = dto.Published.Value;
            if (dto.Collab.HasValue)
                doc.Collab = dto.Collab.Value;
            if (dto.Private.HasValue)
                doc.Private = dto.Private.Value;
            
            doc.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(doc);
            var updated = await _repo.FindByIdAsync(doc.Id);
            return updated == null ? null : _mapper.Map<DocumentResponseDto>(updated);
        }
    }
}
