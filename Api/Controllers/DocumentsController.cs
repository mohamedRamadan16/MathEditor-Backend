using MediatR;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Documents.Queries;
using Api.Application.Documents.Commands;
using Api.Application.Documents.DTOs;
using Api.Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _mediator.Send(new GetAllDocumentsQuery(page, pageSize));
                var response = new {
                    items = result.Items,
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    totalPages = result.TotalPages
                };
                return Ok(new ApiResponse<object>(response));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DocumentResponseDto>>> Get(Guid id)
        {
            try
            {
                var doc = await _mediator.Send(new GetDocumentByIdQuery(id));
                if (doc == null) return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Document not found"));
                return Ok(new ApiResponse<DocumentResponseDto>(doc));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpGet("by-handle/{handle}")]
        public async Task<ActionResult<ApiResponse<DocumentResponseDto>>> GetByHandle(string handle)
        {
            try
            {
                var doc = await _mediator.Send(new GetDocumentByHandleQuery(handle));
                if (doc == null) return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Document not found"));
                return Ok(new ApiResponse<DocumentResponseDto>(doc));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<DocumentResponseDto>>> Create([FromBody] DocumentCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Handle) || userId == null)
                return BadRequest(new ApiResponse<DocumentResponseDto>(null, false, "Invalid document data"));
            try
            {
                var result = await _mediator.Send(new CreateDocumentCommand(dto, userId.Value));
                if (result == null)
                    return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, "Document creation failed."));
                return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<DocumentResponseDto>(result));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpPost("new/{id}")]
        public async Task<ActionResult<ApiResponse<DocumentResponseDto>>> Fork(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<DocumentResponseDto>(null, false, "Unauthorized"));
            try
            {
                var result = await _mediator.Send(new ForkDocumentCommand(id, userId.Value));
                if (result == null)
                    return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Base document not found"));
                return CreatedAtAction(nameof(Get), new { id = result.Id }, new ApiResponse<DocumentResponseDto>(result));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResponse<DocumentResponseDto>>> Update([FromBody] DocumentUpdateDto dto)
        {
            if (dto == null || dto.Id == Guid.Empty)
                return BadRequest(new ApiResponse<DocumentResponseDto>(null, false, "Invalid document data"));
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<DocumentResponseDto>(null, false, "Unauthorized"));
            
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var result = await _mediator.Send(new UpdateDocumentCommand(dto, userId.Value, userEmail));
                if (result == null)
                    return Forbid();
                return Ok(new ApiResponse<DocumentResponseDto>(result));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
                
            try
            {
                var result = await _mediator.Send(new DeleteDocumentCommand(id, userId.Value));
                if (!result.Success)
                {
                    if (result.Error == "Document not found")
                        return NotFound(new ApiResponse<object>(null, false, result.Error));
                    if (result.Error == "Forbidden")
                        return Forbid();
                    return BadRequest(new ApiResponse<object>(null, false, result.Error));
                }
                return Ok(new ApiResponse<object>(new { id = id }));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpPost("{id}/coauthors")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> AddCoauthor(Guid id, [FromBody] string email)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
            try
            {
                var result = await _mediator.Send(new AddCoauthorCommand(id, email, userId.Value));
                if (!result.Success)
                {
                    if (result.Error == "Document not found.")
                        return NotFound(new ApiResponse<object>(null, false, result.Error));
                    if (result.Error == "Forbidden")
                        return Forbid();
                    return BadRequest(new ApiResponse<object>(null, false, result.Error));
                }
                return Ok(new ApiResponse<object>(new { coauthor = result.Coauthor }, true, "Coauthor added."));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpDelete("{id}/coauthors")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> RemoveCoauthor(Guid id, [FromBody] string email)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
            try
            {
                var result = await _mediator.Send(new RemoveCoauthorCommand(id, email, userId.Value));
                if (!result.Success)
                {
                    if (result.Error == "Document not found.")
                        return NotFound(new ApiResponse<object>(null, false, result.Error));
                    if (result.Error == "Forbidden")
                        return Forbid();
                    return BadRequest(new ApiResponse<object>(null, false, result.Error));
                }
                return Ok(new ApiResponse<object>(new { coauthor = result.Coauthor }, true, "Coauthor removed."));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {errorMessage}"));
            }
        }

        [HttpPut("{id}/head")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> UpdateHead(Guid id, [FromBody] Guid newHeadId)
        {
            if (newHeadId == Guid.Empty)
                return BadRequest(new ApiResponse<object>(null, false, "Invalid revision ID."));
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                var result = await _mediator.Send(new UpdateDocumentHeadCommand(id, newHeadId, userId.Value, userEmail));
                if (!result.Success)
                {
                    if (result.StatusCode == 403)
                        return Forbid();
                    if (result.StatusCode == 404)
                        return NotFound(new ApiResponse<object>(null, false, result.Message));
                    return BadRequest(new ApiResponse<object>(null, false, result.Message));
                }
                return Ok(new ApiResponse<object>(new { documentId = id, head = newHeadId }, true, "Document head updated."));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<object>(null, false, $"Error: {errorMessage}"));
            }
        }
    }
}