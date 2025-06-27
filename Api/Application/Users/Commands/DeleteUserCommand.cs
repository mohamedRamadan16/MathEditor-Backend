using Api.Application.Common.Interfaces;
using MediatR;

namespace Api.Application.Users.Commands;

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
    private readonly IUserRepository _userRepository;
    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteUserAsync(request.Id);
        return Unit.Value;
    }
}
