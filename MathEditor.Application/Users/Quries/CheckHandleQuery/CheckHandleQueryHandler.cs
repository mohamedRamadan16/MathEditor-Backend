using MathEditor.Application.Users.DTOs;
using MathEditor.Domain.IRepositories;
using MediatR;
using System.Text.RegularExpressions;

namespace MathEditor.Application.Users.Quries.CheckHandleQuery;

public class CheckHandleQueryHandler : IRequestHandler<CheckHandleQuery, CheckHandleResponseDto>
{
    private readonly IUserRepository _userRepository;

    public CheckHandleQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<CheckHandleResponseDto> Handle(CheckHandleQuery request, CancellationToken cancellationToken)
    {
        var response = new CheckHandleResponseDto();

        try
        {
            if (string.IsNullOrEmpty(request.Handle))
            {
                response.Error = new ErrorDto
                {
                    Title = "Bad Request",
                    Subtitle = "No user handle provided"
                };
                return response;
            }

            var validationError = await ValidateHandle(request.Handle);
            if (validationError != null)
            {
                response.Error = validationError;
                return response;
            }

            response.Data = true;
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
