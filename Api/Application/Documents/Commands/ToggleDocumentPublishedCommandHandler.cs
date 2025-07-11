using MediatR;
using Api.Application.Documents.DTOs;
using Api.Application.Common.Interfaces;
using AutoMapper;
using System;

namespace Api.Application.Documents.Commands
{
    public class ToggleDocumentPublishedCommandHandler : IRequestHandler<ToggleDocumentPublishedCommand, DocumentResponseDto?>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        
        public ToggleDocumentPublishedCommandHandler(IDocumentRepository documentRepository, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _mapper = mapper;
        }
        
        public async Task<DocumentResponseDto?> Handle(ToggleDocumentPublishedCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"ToggleDocumentPublishedCommandHandler: Processing document {request.DocumentId}, published: {request.Published}, userId: {request.UserId}");
            
            var document = await _documentRepository.FindByIdAsync(request.DocumentId);
            if (document == null)
            {
                Console.WriteLine($"Document not found: {request.DocumentId}");
                return null;
            }
            
            // Check if user is the author
            if (document.AuthorId != request.UserId)
            {
                Console.WriteLine($"User {request.UserId} is not the author of document {request.DocumentId}. Author is {document.AuthorId}");
                return null;
            }
            
            Console.WriteLine($"Updating document {request.DocumentId} published status from {document.Published} to {request.Published}");
            
            // Update published status
            document.Published = request.Published;
            document.UpdatedAt = DateTime.UtcNow;
            
            var updatedDocument = await _documentRepository.UpdateAsync(document);
            Console.WriteLine($"Document {request.DocumentId} successfully updated");
            
            return _mapper.Map<DocumentResponseDto>(updatedDocument);
        }
    }
}
