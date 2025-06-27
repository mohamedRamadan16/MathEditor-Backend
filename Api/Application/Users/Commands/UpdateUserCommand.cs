using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using MediatR;

namespace Api.Application.Users.Commands;

public class UpdateUserCommand : IRequest<User>
{
    public Guid Id { get; }
    public User User { get; }
    public UpdateUserCommand(Guid id, User user)
    {
        Id = id;
        User = user;
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userRepository.UpdateUserAsync(request.Id, request.User);
    }
}
