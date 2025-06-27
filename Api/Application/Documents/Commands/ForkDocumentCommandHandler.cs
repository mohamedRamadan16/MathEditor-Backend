using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Api.Application.Documents.DTOs;
using Api.Application.Common.Interfaces;
using AutoMapper;
using Api.Domain.Entities;
using System;

namespace Api.Application.Documents.Commands
{
    public class ForkDocumentCommandHandler : IRequestHandler<ForkDocumentCommand, DocumentResponseDto?>
    {
        private readonly IDocumentRepository _repo;
        private readonly IMapper _mapper;
        public ForkDocumentCommandHandler(IDocumentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<DocumentResponseDto?> Handle(ForkDocumentCommand request, CancellationToken cancellationToken)
        {
            var baseDoc = await _repo.FindByIdAsync(request.BaseDocumentId);
            if (baseDoc == null) return null;
            var fork = new Document
            {
                Id = Guid.NewGuid(),
                Name = baseDoc.Name + " (fork)",
                Handle = null,
                Head = baseDoc.Head,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AuthorId = request.UserId,
                Published = false,
                Collab = baseDoc.Collab,
                Private = baseDoc.Private,
                BaseId = baseDoc.Id
            };
            await _repo.CreateAsync(fork);
            var created = await _repo.FindByIdAsync(fork.Id);
            return created == null ? null : _mapper.Map<DocumentResponseDto>(created);
        }
    }
}
