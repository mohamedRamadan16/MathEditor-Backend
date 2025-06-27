using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Documents.Commands
{
    public class UpdateDocumentHeadCommandHandler : IRequestHandler<UpdateDocumentHeadCommand, UpdateDocumentHeadResult>
    {
        private readonly IDocumentRepository _docRepo;
        public UpdateDocumentHeadCommandHandler(IDocumentRepository docRepo)
        {
            _docRepo = docRepo;
        }
        public async Task<UpdateDocumentHeadResult> Handle(UpdateDocumentHeadCommand request, CancellationToken cancellationToken)
        {
            var doc = await _docRepo.FindByIdAsync(request.DocumentId);
            if (doc == null)
                return new UpdateDocumentHeadResult { Success = false, StatusCode = 404, Message = "Document not found." };
            var isCoauthor = doc.Coauthors.Any(ca =>
                (ca.User != null && ca.User.Id == request.UserId) ||
                (!string.IsNullOrEmpty(request.UserEmail) && ca.UserEmail == request.UserEmail)
            );
            if (doc.AuthorId != request.UserId && !isCoauthor)
                return new UpdateDocumentHeadResult { Success = false, StatusCode = 403, Message = "Forbidden: Not document author or coauthor." };
            var revision = doc.Revisions?.FirstOrDefault(r => r.Id == request.NewHeadId);
            if (revision == null)
                return new UpdateDocumentHeadResult { Success = false, StatusCode = 400, Message = "Revision does not exist or does not belong to this document." };
            doc.Head = request.NewHeadId;
            doc.UpdatedAt = DateTime.UtcNow;
            await _docRepo.UpdateAsync(doc);
            return new UpdateDocumentHeadResult { Success = true, StatusCode = 200, Message = "Document head updated." };
        }
    }
}
