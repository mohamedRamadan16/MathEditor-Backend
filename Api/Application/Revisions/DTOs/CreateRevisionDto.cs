namespace Api.Application.Revisions.DTOs
{
    public class CreateRevisionDto
    {
        public Guid DocumentId { get; set; }
        public string Data { get; set; } = null!;
    }
}