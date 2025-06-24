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
using Api.Application.Documents.Queries;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RevisionsController(IMediator mediator) => _mediator = mediator;

        // Extract user from JWT claims
        private User? GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var handle = User.FindFirst("handle")?.Value;
            var name = User.FindFirst("name")?.Value;
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

        // Helper to get current user ID from JWT
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        private static RevisionResponseDto MapToDto(Revision rev) => new RevisionResponseDto
        {
            Id = rev.Id,
            DocumentId = rev.DocumentId,
            CreatedAt = rev.CreatedAt,
            Data = rev.Data,
            Author = new AuthorDto
            {
                Id = rev.Author.Id,
                Handle = rev.Author.Handle,
                Name = rev.Author.Name,
                Image = rev.Author.Image
            }
        };

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RevisionResponseDto>>> Get(Guid id)
        {
            try
            {
                var rev = await _mediator.Send(new GetRevisionByIdQuery(id));
                if (rev == null)
                    return NotFound(new ApiResponse<RevisionResponseDto>(null, false, "Document Revision not found"));
                return Ok(new ApiResponse<RevisionResponseDto>(MapToDto(rev)));
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

                // Ensure DocumentId exists and load coauthors
                var document = await _mediator.Send(new GetDocumentByIdQuery(dto.DocumentId));
                if (document == null)
                    return BadRequest(new ApiResponse<RevisionResponseDto>(null, false, "Document does not exist"));
                // Only author or coauthor can add a revision
                if (string.IsNullOrWhiteSpace(user.Email))
                    return StatusCode(500, new ApiResponse<RevisionResponseDto>(null, false, "Current user email is missing. Cannot check coauthor permissions."));
                var userEmail = user.Email.Trim().ToLowerInvariant();
                var isCoauthor = document.Coauthors != null && document.Coauthors.Any(ca => ca.UserEmail == userEmail);
                if (document.AuthorId != user.Id && !isCoauthor)
                    return Forbid();

                var rev = new Revision
                {
                    Id = dto.Id != null && dto.Id != Guid.Empty ? dto.Id.Value : Guid.NewGuid(),
                    DocumentId = dto.DocumentId,
                    CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
                    Data = dto.Data,
                    AuthorId = user.Id
                };
                var created = await _mediator.Send(new CreateRevisionCommand(rev));
                // Reload the revision with Author navigation property populated
                var createdWithAuthor = await _mediator.Send(new GetRevisionByIdQuery(created.Id));
                return Ok(new ApiResponse<RevisionResponseDto>(MapToDto(createdWithAuthor)));
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
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
                // Load revision with author and document coauthors
                var rev = await _mediator.Send(new GetRevisionByIdQuery(id));
                if (rev == null)
                    return NotFound(new ApiResponse<object>(null, false, "Revision not found"));
                // Get document with coauthors
                var doc = rev.Document;
                if (doc == null)
                    return BadRequest(new ApiResponse<object>(null, false, "Parent document not found"));
                var isCoauthor = doc.Coauthors != null && doc.Coauthors.Any(ca => ca.User != null && ca.User.Id == userId);
                // Only revision author or document coauthor can delete
                if (rev.AuthorId != userId && !isCoauthor)
                    return Forbid();
                var deleted = await _mediator.Send(new DeleteRevisionCommand(id));
                if (deleted == null)
                    return NotFound(new ApiResponse<object>(null, false, "Revision not found"));
                return Ok(new ApiResponse<object>(new { id = id, documentId = deleted.DocumentId }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {ex.Message}"));
            }
        }
    }
}
