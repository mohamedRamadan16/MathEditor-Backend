namespace Api.Domain.Entities
{
    public class Revision
    {
        public Guid Id { get; set; }
        public string Data { get; set; } = null!; // JSON
        public DateTime CreatedAt { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;
    }
}
