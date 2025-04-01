using MathEditor.Domain.Entities;
using MathEditor.Domain.IRepositories;
using MathEditor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace MathEditor.Infrastructure.Repositories;

public class RevisionRepository : IRevisionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10); // Cache duration of 10 minutes

    public RevisionRepository(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Revision?> FindRevisionById(Guid id)
    {
        return await _context.Revisions
            .AsNoTracking()
            .Include(r => r.Document)
            .Include(r => r.Author)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Revision?> GetCachedRevision(Guid id)
    {
        string cacheKey = $"revision_{id}";
        if (!_cache.TryGetValue(cacheKey, out Revision? revision))
        {
            revision = await FindRevisionById(id);
            if (revision != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration,
                    SlidingExpiration = TimeSpan.FromMinutes(2) // Reset expiration if accessed within 2 minutes
                };
                _cache.Set(cacheKey, revision, cacheEntryOptions);
            }
        }
        return revision;
    }

    public async Task<string?> FindRevisionAuthorId(Guid id)
    {
        return await _context.Revisions
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => r.AuthorId)
            .FirstOrDefaultAsync();
    }

    public async Task<Revision> CreateRevision(Revision revision)
    {
        if (revision.Id == Guid.Empty)
        {
            throw new ArgumentException("Revision ID must be provided.", nameof(revision));
        }

        revision.CreatedAt = DateTime.UtcNow;
        _context.Revisions.Add(revision);
        await _context.SaveChangesAsync();

        return revision;
    }

    public async Task<Revision> UpdateRevision(Guid id, Revision updateData)
    {
        var revision = await _context.Revisions
            .FirstOrDefaultAsync(r => r.Id == id);

        if (revision == null)
        {
            throw new InvalidOperationException($"Revision with ID {id} not found.");
        }

        _context.Entry(revision).CurrentValues.SetValues(updateData);
        await _context.SaveChangesAsync();

        // Invalidate cache
        _cache.Remove($"revision_{id}");

        return revision;
    }

    public async Task<bool> DeleteRevision(Guid id)
    {
        var revision = await _context.Revisions
            .FirstOrDefaultAsync(r => r.Id == id);

        if (revision == null)
        {
            return false;
        }

        _context.Revisions.Remove(revision);
        await _context.SaveChangesAsync();

        // Invalidate cache
        _cache.Remove($"revision_{id}");

        return true;
    }
}
