using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Api.Application.Documents.Queries;
using Api.Application.Documents.Commands;
using Api.Application.Documents.DTOs;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _db;
        public DocumentsController(IMediator mediator, ApplicationDbContext db)
        {
            _mediator = mediator;
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                var query = _db.Documents
                    .Include(d => d.Author)
                    .Include(d => d.Revisions).ThenInclude(r => r.Author)
                    .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                    .Where(d => d.Published);
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                var docs = await query
                    .OrderByDescending(d => d.UpdatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var result = docs.Select(MapToDto).ToList();
                var response = new {
                    items = result,
                    page,
                    pageSize,
                    totalCount,
                    totalPages
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
                var doc = await _db.Documents
                    .Include(d => d.Author)
                    .Include(d => d.Revisions).ThenInclude(r => r.Author)
                    .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                    .FirstOrDefaultAsync(d => d.Id == id);
                if (doc == null) return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Document not found"));
                return Ok(new ApiResponse<DocumentResponseDto>(MapToDto(doc)));
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
                var doc = await _db.Documents
                    .Include(d => d.Author)
                    .Include(d => d.Revisions).ThenInclude(r => r.Author)
                    .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                    .FirstOrDefaultAsync(d => d.Handle == handle);
                if (doc == null) return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Document not found"));
                return Ok(new ApiResponse<DocumentResponseDto>(MapToDto(doc)));
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
                // Create Document
                var doc = new Document
                {
                    Id = Guid.NewGuid(),
                    Handle = dto.Handle,
                    Name = dto.Name,
                    Head = dto.Head ?? Guid.NewGuid(),
                    CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
                    UpdatedAt = dto.UpdatedAt ?? DateTime.UtcNow,
                    AuthorId = userId.Value,
                    Published = dto.Published,
                    Collab = dto.Collab,
                    Private = dto.Private,
                    BaseId = dto.BaseId
                };
                // Add coauthors
                if (dto.Coauthors != null)
                {
                    doc.Coauthors = new List<DocumentCoauthor>();
                    foreach (var email in dto.Coauthors)
                    {
                        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                        if (user == null)
                        {
                            user = new User { Id = Guid.NewGuid(), Email = email, Name = email.Split('@')[0], CreatedAt = DateTime.UtcNow };
                            _db.Users.Add(user);
                        }
                        doc.Coauthors.Add(new DocumentCoauthor { DocumentId = doc.Id, UserEmail = email, User = user, CreatedAt = DateTime.UtcNow });
                    }
                }
                // Add initial revision
                if (dto.InitialRevision != null)
                {
                    doc.Revisions = new List<Revision>();
                    var rev = new Revision
                    {
                        Id = dto.InitialRevision.Id ?? Guid.NewGuid(),
                        Data = dto.InitialRevision.Data,
                        CreatedAt = dto.InitialRevision.CreatedAt ?? DateTime.UtcNow,
                        AuthorId = userId.Value, // Always set to current user
                        DocumentId = doc.Id
                    };
                    doc.Head = rev.Id;
                    doc.Revisions.Add(rev);
                }
                _db.Documents.Add(doc);
                await _db.SaveChangesAsync();
                var created = await _db.Documents
                    .Include(d => d.Author)
                    .Include(d => d.Revisions).ThenInclude(r => r.Author)
                    .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                    .FirstOrDefaultAsync(d => d.Id == doc.Id);
                if (created == null) return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, "Document creation failed."));
                return CreatedAtAction(nameof(Get), new { id = doc.Id }, new ApiResponse<DocumentResponseDto>(MapToDto(created)));
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
            try
            {
                var baseDoc = await _db.Documents.Include(d => d.Revisions).FirstOrDefaultAsync(d => d.Id == id);
                if (baseDoc == null) return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Base document not found"));
                var fork = new Document
                {
                    Id = Guid.NewGuid(),
                    Name = baseDoc.Name + " (fork)",
                    Handle = null,
                    Head = baseDoc.Head,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AuthorId = baseDoc.AuthorId,
                    Published = false,
                    Collab = baseDoc.Collab,
                    Private = baseDoc.Private,
                    BaseId = baseDoc.Id
                };
                _db.Documents.Add(fork);
                await _db.SaveChangesAsync();
                var created = await _db.Documents
                    .Include(d => d.Author)
                    .Include(d => d.Revisions).ThenInclude(r => r.Author)
                    .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                    .FirstOrDefaultAsync(d => d.Id == fork.Id);
                if (created == null) return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, "Fork creation failed."));
                return CreatedAtAction(nameof(Get), new { id = fork.Id }, new ApiResponse<DocumentResponseDto>(MapToDto(created)));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, $"Error: {errorMessage}"));
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<ApiResponse<DocumentResponseDto>>> Update([FromBody] DocumentUpdateDto dto)
        {
            if (dto == null || dto.Id == Guid.Empty)
                return BadRequest(new ApiResponse<DocumentResponseDto>(null, false, "Invalid document data"));
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new ApiResponse<DocumentResponseDto>(null, false, "Unauthorized"));
                var doc = await _db.Documents
                    .Include(d => d.Coauthors)
                    .FirstOrDefaultAsync(d => d.Id == dto.Id);
                if (doc == null) return NotFound(new ApiResponse<DocumentResponseDto>(null, false, "Document not found"));
                // Only author or coauthor can update
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var isCoauthor = doc.Coauthors.Any(ca =>
                    (ca.User != null && ca.User.Id == userId) ||
                    (!string.IsNullOrEmpty(userEmail) && ca.UserEmail == userEmail)
                );
                if (doc.AuthorId != userId && !isCoauthor)
                    return Forbid();
                doc.Name = dto.Name;
                doc.Handle = dto.Handle;
                doc.UpdatedAt = DateTime.UtcNow;
                doc.Published = dto.Published;
                doc.Collab = dto.Collab;
                doc.Private = dto.Private;
                await _db.SaveChangesAsync();
                var updated = await _db.Documents
                    .Include(d => d.Author)
                    .Include(d => d.Revisions).ThenInclude(r => r.Author)
                    .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                    .FirstOrDefaultAsync(d => d.Id == doc.Id);
                if (updated == null)
                    return StatusCode(500, new ApiResponse<DocumentResponseDto>(null, false, "Document update failed."));
                return Ok(new ApiResponse<DocumentResponseDto>(MapToDto(updated)));
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
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
                var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id);
                if (doc == null) return NotFound(new ApiResponse<object>(null, false, "Document not found"));
                // Only author can delete
                if (doc.AuthorId != userId)
                    return Forbid();
                _db.Documents.Remove(doc);
                await _db.SaveChangesAsync();
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
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new ApiResponse<object>(null, false, "Email is required."));
            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!emailValidator.IsValid(email))
                return BadRequest(new ApiResponse<object>(null, false, "Invalid email address."));
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
            var doc = await _db.Documents.Include(d => d.Coauthors).FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return NotFound(new ApiResponse<object>(null, false, "Document not found"));
            if (doc.AuthorId != userId)
                return Forbid();
            var normalizedEmail = email.Trim().ToLowerInvariant();
            if (doc.Coauthors.Any(ca => ca.UserEmail != null && ca.UserEmail.ToLower() == normalizedEmail))
                return BadRequest(new ApiResponse<object>(null, false, "Coauthor already exists."));
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
            if (user == null)
                return BadRequest(new ApiResponse<object>(null, false, "User with this email does not exist. Only registered users can be added as coauthors."));
            doc.Coauthors.Add(new DocumentCoauthor { DocumentId = doc.Id, UserEmail = normalizedEmail, User = user, CreatedAt = DateTime.UtcNow });
            await _db.SaveChangesAsync();
            return Ok(new ApiResponse<object>(new { coauthor = normalizedEmail }, true, "Coauthor added."));
        }

        [HttpDelete("{id}/coauthors")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> RemoveCoauthor(Guid id, [FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new ApiResponse<object>(null, false, "Email is required."));
            var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            if (!emailValidator.IsValid(email))
                return BadRequest(new ApiResponse<object>(null, false, "Invalid email address."));
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
            var doc = await _db.Documents.Include(d => d.Coauthors).FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return NotFound(new ApiResponse<object>(null, false, "Document not found"));
            if (doc.AuthorId != userId)
                return Forbid();
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var coauthor = doc.Coauthors.FirstOrDefault(ca => ca.UserEmail != null && ca.UserEmail.ToLower() == normalizedEmail);
            if (coauthor == null)
                return NotFound(new ApiResponse<object>(null, false, "Coauthor not found."));
            doc.Coauthors.Remove(coauthor);
            await _db.SaveChangesAsync();
            return Ok(new ApiResponse<object>(new { coauthor = normalizedEmail }, true, "Coauthor removed."));
        }

        [HttpPut("{id}/head")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> UpdateHead(Guid id, [FromBody] Guid newHeadId)
        {
            try
            {
                if (newHeadId == Guid.Empty)
                    return BadRequest(new ApiResponse<object>(null, false, "Invalid revision ID."));
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized(new ApiResponse<object>(null, false, "Unauthorized"));
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

        private static DocumentResponseDto MapToDto(Document doc)
        {
            return new DocumentResponseDto
            {
                Id = doc.Id,
                Handle = doc.Handle,
                Name = doc.Name,
                Head = doc.Head,
                CreatedAt = doc.CreatedAt,
                UpdatedAt = doc.UpdatedAt,
                AuthorId = doc.AuthorId,
                Published = doc.Published,
                Collab = doc.Collab,
                Private = doc.Private,
                BaseId = doc.BaseId,
                Author = doc.Author == null ? null : new DocumentUserResponseDto
                {
                    Id = doc.Author.Id,
                    Handle = doc.Author.Handle,
                    Name = doc.Author.Name,
                    Email = doc.Author.Email ?? string.Empty,
                    Image = doc.Author.Image
                },
                Revisions = doc.Revisions?.Select(r => new DocumentRevisionResponseDto
                {
                    Id = r.Id,
                    Data = r.Data,
                    CreatedAt = r.CreatedAt,
                    AuthorId = r.AuthorId,
                    Author = r.Author == null ? null : new DocumentUserResponseDto
                    {
                        Id = r.Author.Id,
                        Handle = r.Author.Handle,
                        Name = r.Author.Name,
                        Email = r.Author.Email ?? string.Empty,
                        Image = r.Author.Image
                    }
                }).ToList(),
                Coauthors = doc.Coauthors?.Where(ca => ca.User != null).Select(ca => new DocumentUserResponseDto
                {
                    Id = ca.User.Id,
                    Handle = ca.User.Handle,
                    Name = ca.User.Name,
                    Email = ca.User.Email ?? string.Empty,
                    Image = ca.User.Image
                }).ToList() ?? new List<DocumentUserResponseDto>()
            };
        }
    }
}
