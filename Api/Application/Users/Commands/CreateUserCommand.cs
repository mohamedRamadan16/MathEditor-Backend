using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Users.Commands;

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
    private readonly IUserRepository _userRepository;
    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.CreateUserAsync(request.User);
    }
}
