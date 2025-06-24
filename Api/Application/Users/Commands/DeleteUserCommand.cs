using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Api.Application.Users.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public Guid Id { get; }
        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly Common.Interfaces.IUserRepository _userRepository;
        public DeleteUserCommandHandler(Common.Interfaces.IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.DeleteUserAsync(request.Id);
            return Unit.Value;
        }
    }
}
