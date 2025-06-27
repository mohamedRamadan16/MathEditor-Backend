using MediatR;
using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Api.Application.Documents.Commands
{
    public class RemoveCoauthorCommandHandler : IRequestHandler<RemoveCoauthorCommand, RemoveCoauthorResult>
    {
        private readonly IDocumentRepository _docRepo;
        public RemoveCoauthorCommandHandler(IDocumentRepository docRepo)
        {
            _docRepo = docRepo;
        }
        public async Task<RemoveCoauthorResult> Handle(RemoveCoauthorCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return new RemoveCoauthorResult { Success = false, Error = "Email is required." };
            var email = request.Email.Trim().ToLowerInvariant();
            var doc = await _docRepo.FindByIdAsync(request.DocumentId);
            if (doc == null)
                return new RemoveCoauthorResult { Success = false, Error = "Document not found." };
            if (doc.AuthorId != request.UserId)
                return new RemoveCoauthorResult { Success = false, Error = "Forbidden" };
            var coauthor = doc.Coauthors.FirstOrDefault(ca => ca.UserEmail != null && ca.UserEmail.ToLower() == email);
            if (coauthor == null)
                return new RemoveCoauthorResult { Success = false, Error = "Coauthor not found." };
            doc.Coauthors.Remove(coauthor);
            await _docRepo.UpdateAsync(doc);
            return new RemoveCoauthorResult { Success = true, Coauthor = email };
        }
    }
}
