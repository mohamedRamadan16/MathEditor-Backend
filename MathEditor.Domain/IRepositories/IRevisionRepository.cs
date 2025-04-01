using MathEditor.Domain.Entities;

namespace MathEditor.Domain.IRepositories;

public interface IRevisionRepository
{
    /// <summary>
    /// Retrieves a revision by its ID.
    /// </summary>
    Task<Revision?> FindRevisionById(Guid id);

    /// <summary>
    /// Retrieves a revision by its ID with caching.
    /// </summary>
    Task<Revision?> GetCachedRevision(Guid id);

    /// <summary>
    /// Retrieves the author ID of a revision by its ID.
    /// </summary>
    Task<string?> FindRevisionAuthorId(Guid id);

    /// <summary>
    /// Creates a new revision.
    /// </summary>
    Task<Revision> CreateRevision(Revision revision);

    /// <summary>
    /// Updates an existing revision by its ID.
    /// </summary>
    Task<Revision> UpdateRevision(Guid id, Revision updateData);

    /// <summary>
    /// Deletes a revision by its ID.
    /// </summary>
    Task<bool> DeleteRevision(Guid id);
}

