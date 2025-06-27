namespace Api.Domain.Entities
{
    public class DocumentCoauthor
    {
        public Guid DocumentId { get; set; }
        public string UserEmail { get; set; } = null!;
        public Document Document { get; set; } = null!;
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
