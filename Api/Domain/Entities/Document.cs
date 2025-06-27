namespace Api.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public string? Handle { get; set; }
        public string Name { get; set; } = null!;
        public Guid Head { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid AuthorId { get; set; }
        public bool Published { get; set; }
        public bool Collab { get; set; }
        public bool Private { get; set; }
        public Guid? BaseId { get; set; }
        public Document? Base { get; set; }
        public ICollection<Document> Forks { get; set; } = new List<Document>();
        public ICollection<Revision> Revisions { get; set; } = new List<Revision>();
        public User Author { get; set; } = null!;
        public ICollection<DocumentCoauthor> Coauthors { get; set; } = new List<DocumentCoauthor>();
    }
}
