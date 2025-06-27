using MediatR;

namespace Api.Application.Revisions.Commands;

public class DeleteRevisionCommand : IRequest<DeleteRevisionResult>
{
    private readonly Guid _revisionId;
    private readonly Guid _userId;
    private readonly string? _userEmail;
    public Guid RevisionId => _revisionId;
    public Guid UserId => _userId;
    public string? UserEmail => _userEmail;
    public DeleteRevisionCommand(Guid revisionId, Guid userId, string? userEmail = null)
    {
        _revisionId = revisionId;
        _userId = userId;
        _userEmail = userEmail;
    }
}
public class DeleteRevisionResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Guid? Id { get; set; }
    public Guid? DocumentId { get; set; }
}
