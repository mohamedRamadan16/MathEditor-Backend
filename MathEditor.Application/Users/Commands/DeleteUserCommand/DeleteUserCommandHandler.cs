using MathEditor.Application.Users.DTOs;
using MathEditor.Domain.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MathEditor.Application.Users.Commands.DeleteUserCommand;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteUserCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<UserResponseDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var response = new UserResponseDto();

        try
        {
            if (!Guid.TryParse(request.Id, out _))
            {
                response.Error = new ErrorDto { Title = "Bad Request", Subtitle = "Invalid user id" };
                return response;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
            {
                response.Error = new ErrorDto { Title = "Unauthenticated", Subtitle = "Please sign in to delete this user" };
                return response;
            }

            var currentUserId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userRepository.FindUserByIdAsync(currentUserId ?? string.Empty);
            if (currentUser == null || currentUser.Disabled || currentUser.Role != "admin")
            {
                response.Error = new ErrorDto { Title = "Unauthorized", Subtitle = "You are not authorized to delete this user" };
                return response;
            }

            var success = await _userRepository.DeleteUserAsync(request.Id);
            if (!success)
            {
                response.Error = new ErrorDto { Title = "Something went wrong", Subtitle = "User deletion failed" };
                return response;
            }

            response.Data = new UserDto { Id = request.Id };
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            response.Error = new ErrorDto
            {
                Title = "Something went wrong",
                Subtitle = "Please try again later"
            };
            return response;
        }
    }
}
