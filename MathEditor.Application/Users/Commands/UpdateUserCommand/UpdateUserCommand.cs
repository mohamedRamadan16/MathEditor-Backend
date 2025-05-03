using MathEditor.Application.Users.DTOs;
using MediatR;

namespace MathEditor.Application.Users.Commands.UpdateUserCommand;

public record UpdateUserCommand(string Id, UserDto UserUpdateInput) : IRequest<UserResponseDto>;
