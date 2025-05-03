using MathEditor.Application.Users.DTOs;
using MediatR;

namespace MathEditor.Application.Users.Quries.CheckHandleQuery;

public record CheckHandleQuery(string Handle) : IRequest<CheckHandleResponseDto>;
