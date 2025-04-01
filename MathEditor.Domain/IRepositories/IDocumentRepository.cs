using MathEditor.Domain.Entities;

namespace MathEditor.Domain.IRepositories;

public interface IDocumentRepository
{
    Task<IEnumerable<Document>> FindPublishedDocuments();
    Task<IEnumerable<Document>> FindDocumentsByAuthorId(string authorId);
    Task<IEnumerable<Document>> FindPublishedDocumentsByAuthorId(string authorId);
    Task<IEnumerable<(string Id, string Name, long Size)>> FindCloudStorageUsageByAuthorId(string authorId);
    Task<IEnumerable<Document>> FindDocumentsByCoauthorId(string authorId);
    Task<IEnumerable<Document>> FindDocumentsByCollaboratorId(string authorId);

    /// <summary>
    /// Retrieves a document for editing by its handle or ID, including the latest revision's data.
    /// </summary>
    Task<Document?> FindEditorDocument(string handle);

    /// <summary>
    /// Retrieves a document by handle or ID, optionally including specific revisions.
    /// </summary>
    Task<Document?> FindUserDocument(string handle, string? revisions = null);

    Task<Document?> CreateDocument(Document document);
    Task<Document?> UpdateDocument(string handle, Document updateData);
    Task<bool> DeleteDocument(string handle);
}

