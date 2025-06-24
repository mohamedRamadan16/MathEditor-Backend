using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Users.Commands
{
    public class CreateUserCommand : IRequest<User>
    {
        public User User { get; }
        public CreateUserCommand(User user)
        {
            User = user;
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly Common.Interfaces.IUserRepository _userRepository;
        public CreateUserCommandHandler(Common.Interfaces.IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.CreateUserAsync(request.User);
        }
    }
}
