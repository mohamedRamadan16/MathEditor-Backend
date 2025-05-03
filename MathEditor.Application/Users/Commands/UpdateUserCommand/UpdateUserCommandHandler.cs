using MathEditor.Application.Users.DTOs;
using MathEditor.Domain.Entities;
using MathEditor.Domain.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace MathEditor.Application.Users.Commands.UpdateUserCommand;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateUserCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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
                response.Error = new ErrorDto { Title = "Unauthenticated", Subtitle = "Please sign in to update your profile" };
                return response;
            }

            var currentUserId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || currentUserId != request.Id)
            {
                response.Error = new ErrorDto { Title = "Unauthorized", Subtitle = "You are not authorized to update this profile" };
                return response;
            }

            var user = await _userRepository.FindUserByIdAsync(request.Id);
            if (user == null || user.Disabled)
            {
                response.Error = new ErrorDto { Title = "Account Disabled", Subtitle = "Account is disabled for violating terms of service" };
                return response;
            }

            if (request.UserUpdateInput == null)
            {
                response.Error = new ErrorDto { Title = "Bad Request", Subtitle = "No update provided" };
                return response;
            }

            var updateData = new ApplicationUser
            {
                Id = request.Id,
                Handle = request.UserUpdateInput.Handle?.ToLower(),
                Name = request.UserUpdateInput.Name,
                Email = request.UserUpdateInput.Email,
                Image = request.UserUpdateInput.Image
            };

            // Validate handle if it has changed
            if (!string.IsNullOrEmpty(updateData.Handle) && updateData.Handle != user.Handle)
            {
                var validationError = await ValidateHandle(updateData.Handle);
                if (validationError != null)
                {
                    response.Error = validationError;
                    return response;
                }
            }

            await _userRepository.UpdateUserAsync(request.Id, updateData);

            var updatedUser = await _userRepository.FindUserByIdAsync(request.Id);
            if (updatedUser == null)
            {
                response.Error = new ErrorDto { Title = "Something went wrong", Subtitle = "User update failed" };
                return response;
            }

            response.Data = new UserDto
            {
                Id = updatedUser.Id,
                Handle = updatedUser.Handle ?? string.Empty,
                Name = updatedUser.Name ?? string.Empty,
                Email = updatedUser.Email ?? string.Empty,
                Image = updatedUser.Image ?? string.Empty
            };

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

    private async Task<ErrorDto?> ValidateHandle(string handle)
    {
        // Check length
        if (handle.Length < 3)
        {
            return new ErrorDto
            {
                Title = "Handle too short",
                Subtitle = "Handle must be at least 3 characters"
            };
        }

        // Check format (letters, numbers, hyphens only)
        if (!Regex.IsMatch(handle, @"^[a-zA-Z0-9-]+$"))
        {
            return new ErrorDto
            {
                Title = "Invalid Handle",
                Subtitle = "Handle must only contain letters, numbers, and hyphens"
            };
        }

        // Check if handle is a UUID
        if (Guid.TryParse(handle, out _))
        {
            return new ErrorDto
            {
                Title = "Invalid Handle",
                Subtitle = "Handle must not be a UUID"
            };
        }

        // Check if handle is already taken
        var existingUser = await _userRepository.FindUserAsync(handle);
        if (existingUser != null)
        {
            return new ErrorDto
            {
                Title = "Handle already taken",
                Subtitle = "Please choose a different handle"
            };
        }

        return null;
    }
}
