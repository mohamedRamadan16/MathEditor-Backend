namespace Api.Application.Revisions.DTOs
{
    public class CreateRevisionDto
    {
        public Guid? Id { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Data { get; set; } = null!;
    }
}