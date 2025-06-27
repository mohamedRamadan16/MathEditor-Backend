using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;
        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Document?> FindByIdAsync(Guid id)
        {
            return await _context.Documents
                .Include(d => d.Author)
                .Include(d => d.Revisions).ThenInclude(r => r.Author)
                .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Document?> FindByHandleAsync(string handle)
        {
            return await _context.Documents
                .Include(d => d.Author)
                .Include(d => d.Revisions).ThenInclude(r => r.Author)
                .Include(d => d.Coauthors).ThenInclude(ca => ca.User)
                .FirstOrDefaultAsync(d => d.Handle == handle);
        }

        public async Task<Document> CreateAsync(Document doc)
        {
            doc.Id = Guid.NewGuid();
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<Document> UpdateAsync(Document doc)
        {
            _context.Documents.Update(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task DeleteAsync(Guid id)
        {
            var doc = await _context.Documents.FindAsync(id);
            if (doc != null)
            {
                _context.Documents.Remove(doc);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(List<Document> Items, int TotalCount)> GetPublishedPagedAsync(
            int page, int pageSize, bool tracked = false
        )
        {
            IQueryable<Document> query = _context.Documents;
            query = query.Where(d => d.Published)
                .Include(d => d.Author)
                .Include(d => d.Revisions).ThenInclude(r => r.Author)
                .Include(d => d.Coauthors).ThenInclude(ca => ca.User);
            if (!tracked)
                query = query.AsNoTracking();
            int totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(d => d.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }
    }
}
