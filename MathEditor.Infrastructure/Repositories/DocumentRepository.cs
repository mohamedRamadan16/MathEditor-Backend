
using MathEditor.Domain.Entities;
using MathEditor.Domain.IRepositories;
using MathEditor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace MathEditor.Infrastructure.Repositories
{
    internal class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Document>> FindPublishedDocuments()
        {
            return await BaseDocumentQuery()
                .Where(d => d.Published)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> FindDocumentsByAuthorId(string authorId)
        {
            var authoredDocuments = await BaseDocumentQuery()
                .Where(d => d.AuthorId == authorId)
                .ToListAsync();

            var coauthoredDocuments = await FindDocumentsByCoauthorId(authorId);
            var collaboratorDocuments = await FindDocumentsByCollaboratorId(authorId);
            var publishedDocuments = await FindPublishedDocuments();

            return authoredDocuments
                .Concat(coauthoredDocuments)
                .Concat(collaboratorDocuments)
                .Concat(publishedDocuments)
                .GroupBy(d => d.Id)
                .Select(g => g.First())
                .OrderByDescending(d => d.UpdatedAt);
        }

        public async Task<IEnumerable<(string Id, string Name, long Size)>> FindCloudStorageUsageByAuthorId(string authorId)
        {
            // Using raw SQL with DATALENGTH for accurate size calculation in SQL Server
            var documentSizes = await _context.Database
                .SqlQueryRaw<(string Id, string Name, long Size)>(
                    "SELECT d.Id, d.Name, SUM(DATALENGTH(d.*) + ISNULL(DATALENGTH(r.*), 0)) as Size " +
                    "FROM Documents d " +
                    "LEFT JOIN Revisions r ON d.Id = r.DocumentId " +
                    "WHERE d.AuthorId = {0} " +
                    "GROUP BY d.Id, d.Name " +
                    "ORDER BY d.UpdatedAt DESC", authorId)
                .ToListAsync();

            return documentSizes;
        }

        public async Task<IEnumerable<Document>> FindPublishedDocumentsByAuthorId(string authorId)
        {
            return await BaseDocumentQuery()
                .Where(d => d.AuthorId == authorId && d.Published)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> FindDocumentsByCoauthorId(string authorId)
        {
            return await _context.DocumentCoauthors
                .Where(dc => dc.UserId == authorId)
                .Include(dc => dc.Document)
                    .ThenInclude(d => d.Author)
                .Include(dc => dc.Document.Revisions)
                    .ThenInclude(r => r.Author)
                .Include(dc => dc.Document.Coauthors)
                    .ThenInclude(dc => dc.User)
                .Select(dc => dc.Document)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> FindDocumentsByCollaboratorId(string authorId)
        {
            return await _context.Revisions
                .Where(r => r.AuthorId == authorId && r.Document.Collab && r.Document.AuthorId != authorId)
                .Include(r => r.Document)
                    .ThenInclude(d => d.Author)
                .Include(r => r.Document.Revisions)
                    .ThenInclude(r => r.Author)
                .Include(r => r.Document.Coauthors)
                    .ThenInclude(dc => dc.User)
                .Select(r => r.Document)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Document?> FindEditorDocument(string handle)
        {
            return await GetDocumentByHandleOrId(handle);
        }

        public async Task<Document?> FindUserDocument(string handle, string? revisions = null)
        {
            bool isUuid = Guid.TryParse(handle, out var id);

            var documentQuery = BaseDocumentQuery();
            var document = await documentQuery
                .FirstOrDefaultAsync(d => (isUuid && d.Id == id) || d.Handle == handle.ToLower());

            if (document == null) return null;

            if (revisions != "all")
            {
                Guid revisionId = document.Head; // Default to Head
                if (!string.IsNullOrEmpty(revisions) && Guid.TryParse(revisions, out var parsedRevId))
                {
                    revisionId = parsedRevId;
                }

                var revision = document.Revisions.FirstOrDefault(r => r.Id == revisionId);
                if (revision != null)
                {
                    document.Revisions = new List<Revision> { revision };
                    document.UpdatedAt = revision.CreatedAt;
                }
            }

            return document;
        }

        public async Task<Document?> CreateDocument(Document document)
        {
            if (document.Id == Guid.Empty)
            {
                throw new ArgumentException("Document ID must be provided.", nameof(document));
            }

            document.CreatedAt = DateTime.UtcNow;
            document.UpdatedAt = DateTime.UtcNow;
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return await FindUserDocument(document.Id.ToString());
        }

        public async Task<Document?> UpdateDocument(string handle, Document updateData)
        {
            var document = await GetDocumentByHandleOrId(handle);
            if (document == null) return null;

            _context.Entry(document).CurrentValues.SetValues(updateData);
            document.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await FindUserDocument(handle, "all");
        }

        public async Task<bool> DeleteDocument(string handle)
        {
            var document = await GetDocumentByHandleOrId(handle);
            if (document == null) return false;

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        // Helper Methods
        private IQueryable<Document> BaseDocumentQuery()
        {
            return _context.Documents
                .Include(d => d.Author)
                .Include(d => d.Revisions)
                    .ThenInclude(r => r.Author)
                .Include(d => d.Coauthors)
                    .ThenInclude(dc => dc.User)
                .OrderByDescending(d => d.UpdatedAt);
        }

        private async Task<Document?> GetDocumentByHandleOrId(string handle)
        {
            var isUuid = Guid.TryParse(handle, out var id);
            return await _context.Documents
                .Include(d => d.Author)
                .Include(d => d.Revisions)
                    .ThenInclude(r => r.Author)
                .Include(d => d.Coauthors)
                    .ThenInclude(dc => dc.User)
                .FirstOrDefaultAsync(d => (isUuid && d.Id == id) || (!isUuid && d.Handle == handle.ToLower()));
        }
    }
}
