using Api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Users.Queries;
using Api.Application.Users.Commands;
using Api.Application.Users.DTOs;
using Api.Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UsersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("{handleOrId}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Get(string handleOrId)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByHandleOrIdQuery(handleOrId));
                if (user == null) return NotFound(new ApiResponse<UserResponseDto>(null, false, "User not found"));
                return Ok(new ApiResponse<UserResponseDto>(_mapper.Map<UserResponseDto>(user)));
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
                return Ok(new ApiResponse<UserResponseDto>(_mapper.Map<UserResponseDto>(user)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (dto == null || id == Guid.Empty)
                return BadRequest(new ApiResponse<UserResponseDto>(null, false, "Invalid user data"));
            try
            {
                // Only allow user to update their own account (or admin)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("admin");
                if (!isAdmin && (!Guid.TryParse(userIdClaim, out var userId) || userId != id))
                    return Forbid();

                var existingUser = await _mediator.Send(new GetUserByHandleOrIdQuery(id.ToString()));
                if (existingUser == null)
                    return NotFound(new ApiResponse<UserResponseDto>(null, false, "User not found"));

                var user = _mapper.Map<User>(dto);
                user.Id = id;
                user.Email = existingUser.Email;
                
                var updated = await _mediator.Send(new UpdateUserCommand(id, user));
                return Ok(new ApiResponse<UserResponseDto>(_mapper.Map<UserResponseDto>(updated)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            try
            {
                // Only allow user to delete their own account (or admin)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                var isAdmin = User.IsInRole("admin");
                if (!isAdmin && (!Guid.TryParse(userIdClaim, out var userId) || userId != id))
                    return Forbid();

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
