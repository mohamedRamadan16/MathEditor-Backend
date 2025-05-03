using MathEditor.Application.Users.DTOs;
using MediatR;

namespace MathEditor.Application.Users.Quries.GetUserByIdQuery;

public record GetUserByIdQuery(string Id) : IRequest<UserResponseDto>;
