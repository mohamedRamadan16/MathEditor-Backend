using MathEditor.Application.Users.DTOs;
using MediatR;

namespace MathEditor.Application.Users.Commands.DeleteUserCommand;

public record DeleteUserCommand(string Id) : IRequest<UserResponseDto>;
