using MathEditor.Application.Users.DTOs;
using MathEditor.Domain.IRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace MathEditor.Application.Users.Quries.GetUserByIdQuery;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<UserResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new UserResponseDto();

        try
        {
            var user = await _userRepository.FindUserAsync(request.Id);
            if (user == null)
            {
                response.Error = new ErrorDto { Title = "User not found" };
                return response;
            }

            response.Data = new UserDto
            {
                Id = user.Id,
                Handle = user.Handle ?? string.Empty,
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Image = user.Image ?? string.Empty
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
}
