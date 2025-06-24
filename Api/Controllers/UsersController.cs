using System;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Users.Queries;
using Api.Application.Users.Commands;
using Api.Application.Users.DTOs;
using Api.Application.Common.DTOs;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        private static UserResponseDto MapToDto(User user) => new UserResponseDto
        {
            Id = user.Id,
            Handle = user.Handle,
            Name = user.Name,
            Email = user.Email,
            Image = user.Image
        };

        [HttpGet("{handleOrId}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Get(string handleOrId)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByHandleOrIdQuery(handleOrId));
                if (user == null) return NotFound(new ApiResponse<UserResponseDto>(null, false, "User not found"));
                return Ok(new ApiResponse<UserResponseDto>(MapToDto(user)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetByEmail(string email)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByEmailQuery(email));
                if (user == null) return NotFound(new ApiResponse<UserResponseDto>(null, false, "User not found"));
                return Ok(new ApiResponse<UserResponseDto>(MapToDto(user)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Update(Guid id, [FromBody] UserResponseDto dto)
        {
            if (dto == null || id == Guid.Empty)
                return BadRequest(new ApiResponse<UserResponseDto>(null, false, "Invalid user data"));
            try
            {
                // Fetch the existing user to preserve the email
                var existingUser = await _mediator.Send(new GetUserByHandleOrIdQuery(id.ToString()));
                if (existingUser == null)
                    return NotFound(new ApiResponse<UserResponseDto>(null, false, "User not found"));

                var user = new User
                {
                    Id = id,
                    Handle = dto.Handle,
                    Name = dto.Name,
                    Email = existingUser.Email, // Preserve the original email
                    Image = dto.Image
                };
                var updated = await _mediator.Send(new UpdateUserCommand(id, user));
                return Ok(new ApiResponse<UserResponseDto>(MapToDto(updated)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteUserCommand(id));
                return Ok(new ApiResponse<object>(new { id = id }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {ex.Message}"));
            }
        }
    }
}
