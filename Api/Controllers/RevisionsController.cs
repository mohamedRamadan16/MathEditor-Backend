using Api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Revisions.Queries;
using Api.Application.Revisions.Commands;
using Api.Application.Revisions.DTOs;
using Api.Application.Common.DTOs;
using RevisionResponseDto = Api.Application.Revisions.DTOs.RevisionResponseDto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public RevisionsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        // Extract user from JWT claims
        private User? GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var handle = User.FindFirst("handle")?.Value;
            var name = User.FindFirst("name")?.Value??"N/A";
            var image = User.FindFirst("image")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return null;
            return new User
            {
                Id = userId,
                Handle = handle,
                Name = name,
                Email = email,
                Disabled = false,
                Image = image
            };
        }
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RevisionResponseDto>>> Get(Guid id)
        {
            try
            {
                var rev = await _mediator.Send(new GetRevisionByIdQuery(id));
                if (rev == null)
                    return NotFound(new ApiResponse<RevisionResponseDto>(null, false, "Document Revision not found"));
                var dto = _mapper.Map<RevisionResponseDto>(rev);
                return Ok(new ApiResponse<RevisionResponseDto>(dto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<RevisionResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<RevisionResponseDto>>> Create([FromBody] CreateRevisionDto dto)
        {
            if (dto == null || dto.DocumentId == Guid.Empty)
                return BadRequest(new ApiResponse<RevisionResponseDto>(null, false, "Invalid revision data"));
            
            try
            {
                var user = GetCurrentUser();
                if (user == null)
                    return Unauthorized(new ApiResponse<RevisionResponseDto>(null, false, "Unauthorized: Please sign in to save your revision to the cloud"));
                
                if (user.Disabled)
                    return StatusCode(403, new ApiResponse<RevisionResponseDto>(null, false, "Account Disabled: Account is disabled for violating terms of service"));

                var result = await _mediator.Send(new CreateRevisionCommand(dto, user.Id, user.Email, user.Name));
                if (result == null)
                    return Forbid();
                
                return Ok(new ApiResponse<RevisionResponseDto>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<RevisionResponseDto>(null, false, $"Error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            var user = GetCurrentUser();
            if (user == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
            try
            {
                var result = await _mediator.Send(new DeleteRevisionCommand(id, user.Id, user.Email));
                if (!result.Success)
                {
                    if (result.Error == "Revision not found.")
                        return NotFound(new ApiResponse<object>(null, false, result.Error));
                    if (result.Error == "Parent document not found.")
                        return BadRequest(new ApiResponse<object>(null, false, result.Error));
                    if (result.Error == "Forbidden")
                        return Forbid();
                    return BadRequest(new ApiResponse<object>(null, false, result.Error));
                }
                return Ok(new ApiResponse<object>(new { id = result.Id, documentId = result.DocumentId }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {ex.Message}"));
            }
        }
    }
}
