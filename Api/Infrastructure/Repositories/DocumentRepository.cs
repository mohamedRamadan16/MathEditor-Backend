using System;
using System.Threading.Tasks;
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
                .Include(d => d.Revisions)
                .Include(d => d.Coauthors)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Document?> FindByHandleAsync(string handle)
        {
            return await _context.Documents
                .Include(d => d.Author)
                .Include(d => d.Revisions)
                .Include(d => d.Coauthors)
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
    }
}
