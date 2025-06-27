using MediatR;
using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Api.Application.Documents.Commands
{
    public class AddCoauthorCommandHandler : IRequestHandler<AddCoauthorCommand, AddCoauthorResult>
    {
        private readonly IDocumentRepository _docRepo;
        private readonly IUserRepository _userRepo;
        public AddCoauthorCommandHandler(IDocumentRepository docRepo, IUserRepository userRepo)
        {
            _docRepo = docRepo;
            _userRepo = userRepo;
        }
        public async Task<AddCoauthorResult> Handle(AddCoauthorCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return new AddCoauthorResult { Success = false, Error = "Email is required." };

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepo.FindUserByEmailAsync(email);
            if (user == null)
                return new AddCoauthorResult { Success = false, Error = "User with this email does not exist. Only registered users can be added as coauthors." };  

            var doc = await _docRepo.FindByIdAsync(request.DocumentId);
            if (doc == null)
                return new AddCoauthorResult { Success = false, Error = "Document not found." };
            if (doc.AuthorId != request.UserId)
                return new AddCoauthorResult { Success = false, Error = "Forbidden" };
            if (doc.Coauthors.Any(ca => ca.UserEmail != null && ca.UserEmail.ToLower() == email))
                return new AddCoauthorResult { Success = false, Error = "Coauthor already exists." };
            doc.Coauthors.Add(new DocumentCoauthor { DocumentId = doc.Id, UserEmail = email, User = user, CreatedAt = DateTime.UtcNow });
            
            await _docRepo.UpdateAsync(doc);
            return new AddCoauthorResult { Success = true, Coauthor = email };
        }
    }
}
