using System;
using System.Threading.Tasks;
using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public class RevisionRepository : IRevisionRepository
    {
        private readonly ApplicationDbContext _context;
        public RevisionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Revision?> FindByIdAsync(Guid id)
        {
            return await _context.Revisions
                .Include(r => r.Document)
                .Include(r => r.Author)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Revision> CreateAsync(Revision revision)
        {
            revision.Id = Guid.NewGuid();
            _context.Revisions.Add(revision);
            await _context.SaveChangesAsync();
            return revision;
        }

        public async Task<Revision> UpdateAsync(Revision revision)
        {
            _context.Revisions.Update(revision);
            await _context.SaveChangesAsync();
            return revision;
        }

        public async Task DeleteAsync(Guid id)
        {
            var revision = await _context.Revisions.FindAsync(id);
            if (revision != null)
            {
                _context.Revisions.Remove(revision);
                await _context.SaveChangesAsync();
            }
        }
    }
}
