using MediatR;
using Api.Application.Common.Interfaces;

namespace Api.Application.Revisions.Commands;

public class DeleteRevisionCommandHandler : IRequestHandler<DeleteRevisionCommand, DeleteRevisionResult>
{
    private readonly IRevisionRepository _revRepo;
    private readonly IDocumentRepository _docRepo;
    public DeleteRevisionCommandHandler(IRevisionRepository revRepo, IDocumentRepository docRepo)
    {
        _revRepo = revRepo;
        _docRepo = docRepo;
    }

    public async Task<DeleteRevisionResult> Handle(DeleteRevisionCommand request, CancellationToken cancellationToken)
    {
        var revision = await _revRepo.FindByIdAsync(request.RevisionId);
        if (revision == null)
            return new DeleteRevisionResult { Success = false, Error = "Revision not found." };
        var doc = revision.Document;
        if (doc == null)
            return new DeleteRevisionResult { Success = false, Error = "Parent document not found." };
        var isCoauthor = doc.Coauthors != null &&
            doc.Coauthors.Any(ca =>
                (!string.IsNullOrEmpty(request.UserEmail) && ca.UserEmail.ToLowerInvariant().Trim() == request.UserEmail.ToLowerInvariant().Trim()) ||
                (ca.User != null && ca.User.Id == request.UserId)
            );

        await _revRepo.DeleteAsync(request.RevisionId);
        // If the deleted revision was the head, update the document's head
        if (doc.Head == revision.Id)
        {
            var updatedDoc = await _docRepo.FindByIdAsync(doc.Id);
            if (updatedDoc != null)
            {
                var latestRevision = updatedDoc.Revisions
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefault();
                if (latestRevision != null)
                    updatedDoc.Head = latestRevision.Id;
                else
                    updatedDoc.Head = Guid.Empty;
                await _docRepo.UpdateAsync(updatedDoc);
            }
        }
        return new DeleteRevisionResult { Success = true, Id = revision.Id, DocumentId = revision.DocumentId };
    }
}
