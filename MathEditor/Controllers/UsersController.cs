using MathEditor.Application.Users.DTOs;
using MathEditor.Application.Users.Quries;
using MathEditor.Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MathEditor.Application.Users.Commands.DeleteUserCommand;
using MathEditor.Application.Users.Commands.UpdateUserCommand;
using MathEditor.Application.Users.Quries.CheckHandleQuery;
using MathEditor.Application.Users.Quries.GetUserByIdQuery;

namespace MathEditor.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var response = await _mediator.Send(new GetUserByIdQuery(id));
            if (response.Error != null)
            {
                return StatusCode(response.Error.Title switch
                {
                    "User not found" => 404,
                    _ => 500
                }, response.Error);
            }
            return Ok(response);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userUpdateInput)
        {
            var response = await _mediator.Send(new UpdateUserCommand(id, userUpdateInput));
            if (response.Error != null)
            {
                return StatusCode(response.Error.Title switch
                {
                    "Bad Request" => 400,
                    "Unauthenticated" => 401,
                    "Unauthorized" => 403,
                    "Account Disabled" => 403,
                    _ => 500
                }, response.Error);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var response = await _mediator.Send(new DeleteUserCommand(id));
            if (response.Error != null)
            {
                return StatusCode(response.Error.Title switch
                {
                    "Bad Request" => 400,
                    "Unauthenticated" => 401,
                    "Unauthorized" => 403,
                    _ => 500
                }, response.Error);
            }
            return Ok(response);
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckHandle([FromQuery] string handle)
        {
            var response = await _mediator.Send(new CheckHandleQuery(handle));
            if (response.Error != null)
            {
                return StatusCode(response.Error.Title switch
                {
                    "Bad Request" => 400,
                    _ => 500
                }, response.Error);
            }
            return Ok(response);
        }
    }
}